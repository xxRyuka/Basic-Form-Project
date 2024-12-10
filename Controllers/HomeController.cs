using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FormApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Threading.Tasks;
using System.Formats.Asn1;
using System.IO.Pipelines;

namespace FormApp.Controllers;

public class HomeController : Controller
{



    [HttpGet] // Zaten Defaultu get
    public IActionResult Index(string searchString, string? category)
    {
        var Products = Repository.Products;

        if (!String.IsNullOrEmpty(searchString))
        {
            // Eğerki search string null değil ise
            //item.name içersinde searchString ifadesi içeriyorsa bunu listele ve bir alt küme listesi olarak dön 
            ViewBag.sea = searchString;
            Products = Products.Where(item => item.Name!.ToLower().Contains(searchString.ToLower())).ToList();
        }


        if (!String.IsNullOrEmpty(category))
        {
            ViewBag.Console = category;
            Products = Products.Where(c => c.CategoryID == int.Parse(category)).ToList();
        }

        var model = new ProductViewModel()
        {
            Products = Products,
            Categories = Repository.Categories,
            SelectedCategory = category

        };
        return View(model);
    }


    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(items: Repository.Categories, "CategoryID", "NameCategory");
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Create(Product model, IFormFile imgFile)
    {
        ViewBag.Categories = new SelectList(items: Repository.Categories, "CategoryID", "NameCategory");


        if (imgFile == null)
        { // Dosya yoksa hayvan gibi error mesaji yicen biliyon mu? biliyon oyüzden önce kontrolunu saglamalısın

            ModelState.AddModelError("", "img nerde aq");
            return View(model); // Tekrardan create sayfasini gönderiyoz
        }
        else // Geçerliyse yani dosya varsa önce bu kontrolu yapmam gerekiyordu
        {

            // Simdi atatürkün dediği gibi step by step anlatcam numara sirasiylan oku

            //
            var AllowedExtension = new[] { ".png", ".jpg", ".jpeg" }; // Sadece izin verilen dosya uzantilari   


            // buda bize formla gelen dosyanin uzantisini söylicek output : .png   .jpg cart curt
            var extension = Path.GetExtension(imgFile.FileName)?.ToLower(); // Uzantiyi alir abc.jpg  

            //  Bu her dosyaya benzersiz bir isim verecek çakısmaları ve hayvan gibi error msgler görmemek icin 
            var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}"); // Rastgele isim olusturduk guid ile bunada dosyanin uzantisini verdik


            // 1-) Önce dosyayi kaydedeceğimiz konumu seciyoruz sadece syntaxtan ibaret 
            // randomFileName sonradan dahil olucak dosyalar cakısmasin diye
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName); // dosya yolu işlemleri 



            if (imgFile != null || imgFile!.Length == 0)
            {
                // Dosya uzanti kontrolu yapılıyor burda egerki izin verilen formatin disidnaysa siktiri cekiyoz
                if (!AllowedExtension.Contains(extension))
                {
                    ModelState.AddModelError("", "Dosya Formatiniz png,jpg veya jpeg olmalidir ");
                    return View(model);
                }
            }

            // Gerekli kontrolleri geçtikten sonra
            if (ModelState.IsValid)
            {

                // olasi hatalar için try catch kullanıyorum


                try
                {


                    // using kullaniyom cünkü işim bitince dosyayi serbest bırakacak performans ve güvenlik acısından gerekli 
                    //ayrica bunu kullanmazsan cogu seyi manuel yapman gerekecek
                    using (var stream = new FileStream(path, FileMode.Create))
                    {  // dosya akısına dosya yolunu verdik birde mode olarak yoksa yarat dedik

                        await imgFile!.CopyToAsync(stream); // asenkron olarak kopyaliyor yani bu dosya kopyalanirken clienti dondurmayacak arkada işlem yapmaya devam edebileceğim
                    }
                }
                catch (Exception ea) // Sıkıntı cıkarsa loglari görmek istiyorum ve kullanıcıya tekrardan viewi gösteriyor
                {
                    Console.WriteLine(ea.Message);
                    ModelState.AddModelError("", $"Dosya yükleme sırasında bir hata oluştu: {ea.Message}");
                    return View(model);
                }

                model.Image = randomFileName;



                Repository.AddProduct(model);
                return RedirectToAction("Index");
            }
        }
        if (!ModelState.IsValid)
        {
            ViewBag.isValid = "bg-danger";
        }
        return View(model);
    }

    public IActionResult Edit(int? id)
    {

        if (id == null)
        {
            return NotFound();
        }

        var model = Repository.Products.FirstOrDefault(item => item.ProductID == id);

        if (model == null)
        {
            return NotFound();
        }


        ViewBag.Categories = new SelectList(items: Repository.Categories, "CategoryID", "NameCategory");
        return View(model);
    }



    [HttpPost]
    public async Task<IActionResult> Edit(int id, Product newProduct, IFormFile? imgFile)
    {

        if (id != newProduct.ProductID)
        {
            return NotFound();
        }


        if (imgFile != null)
        {


            var extension = Path.GetExtension(imgFile.FileName)?.ToLower();
            var NFileName = String.Format($"{Guid.NewGuid().ToString()}{extension}");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", NFileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await imgFile.CopyToAsync(stream);
            }
            newProduct.Image = NFileName;
        }

        Repository.EditProduct(newProduct);
        return RedirectToAction("index");
    }



    public IActionResult Delete(int? id){

        var entity = Repository.Products.FirstOrDefault(c => c.ProductID ==id);

        if (entity!=null && id!=null)
        {
            return View("ConfrimDelete",entity);

        }
        else
        {
            return NotFound();            
        }

    }


    [HttpPost]
    public IActionResult Delete(int? id,int? ProductID){
        if (id!=null)
        {
            var entity = Repository.Products.FirstOrDefault(c=> c.ProductID == ProductID);
            if (entity!=null)
            {
            Repository.Products.Remove(entity);
            return RedirectToAction("Index");
            }
        }

        return View("Index");
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
