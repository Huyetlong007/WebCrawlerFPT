/// <summary>
/// Copyright by Tin Trinh @ 2020
/// Project: Web Automation using Selenium WebDriver (ChromeDriver)
/// Background knowledge: Need to have
/// - Basic C# programming skill
/// - Skill to process data using Regular Expressions in C#
/// Guide:
/// - First, please update Nuget packages (Selenium WebDriver, Selenium Chrome Driver), Google Chrome to the latest version
/// - Run the demo code below to make sure everything works fine.
/// - Contact me at trinhtrongtinpp@gmail.com
/// </summary>

using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;
namespace WebAutomation
{
    public class Crawler
    {
        //Browser
        private IWebDriver browser;

        //Constructor
        public Crawler() 
        { }

        /// <summary>
        /// Run crawler
        /// </summary>
        /// <param name="productLinks">List of product links need to be crawled</param>
        /// <returns></returns>
        public List<Models.Product> Run(List<string> productLinks)
        {
            //Create new browser instance using IWebDriver
            browser = new ChromeDriver();

            //Create list of product for saving crawled data
            List<Models.Product> listProducts = new List<Models.Product>();

            //Get product information
            //  Iterate though each product link to get product information
            foreach (var link in productLinks)
            {
                var product = GetProductInformation(link);
                listProducts.Add(product);
            }

            //Close browser
            browser.Close();

            return listProducts;
        }

        /// <summary>
        /// Get product information
        /// </summary>
        /// <param name="productLink">Example: https://www.jomashop.com/tissot-watch-t0064071603300.html</param>
        private Models.Product GetProductInformation(string productLink)
        {
            //Create product to save crawled data
            Models.Product product = new Models.Product();

            //Redirect to site by URL
            //browser.Navigate().GoToUrl("https://www.jomashop.com/tissot-watch-t0064071603300.html");
            browser.Navigate().GoToUrl(productLink);
             
            //Sử dụng try catch để over lỗi

            //Lấy loại (Type) Dựa trên breadcumb
            try
            {
            var elementtype = browser.FindElement(By.CssSelector("[class=\"mc-brea\"]"));
            string breadcumb = elementtype.GetAttribute("innerHTML");
            string temp = Regex.Match(breadcumb, "<ul>.*?</li>", RegexOptions.Singleline).Value;
            breadcumb = breadcumb.Replace(temp, "").Trim();//Replace Trang chủ
            temp = Regex.Match(breadcumb, "<li>.*?</li>", RegexOptions.Singleline).Value; 
            breadcumb = breadcumb.Replace(temp, "").Trim();//Replace Máy đổi trả
            temp = Regex.Match(breadcumb, "<li>.*?</li>", RegexOptions.Singleline).Value;
            product.Type = Regex.Match(temp, "<li><a.*?>(.*?)</a></li>", RegexOptions.Singleline).Groups[1].Value;//Lấy giá trị Type
            breadcumb = breadcumb.Replace(temp, "").Trim(); //Replace Type
            
            //Lấy Brand dựa trên link trên (Brand Attribute 1)
            product.Attribute1Value = Regex.Match(breadcumb, "<li><a.*?>(.*?)</a></li>", RegexOptions.Singleline).Groups[1].Value;//Lấy giá trị Brand
            }
            catch (Exception e)
            { Console.WriteLine(productLink + "\nType & Brand went wrong."); }

            //Lấy SKU (SKU)
            try
            {
            var element0 = browser.FindElement(By.CssSelector("[class=\"mc-ctname\"]"));
            var SKU = element0.GetAttribute("innerHTML");
            SKU = Regex.Match(SKU, "<span>(.*?)</span>", RegexOptions.Singleline).Groups[1].Value;
            product.SKU = Regex.Replace(SKU, "\\W", "").Trim();
            }
            catch (Exception e)
            {Console.WriteLine(productLink + "\nSKU went wrong.");}

            //Lấy tên (Name)
            try
            {
            var element1 = browser.FindElement(By.CssSelector("[class=\"mc-ctname\"]"));
            var name = element1.GetAttribute("innerHTML");
            product.Name = Regex.Replace(name, "<span>.*?</span>", "").Trim();
            }
            catch (Exception e)
            {Console.WriteLine(productLink + "\nName went wrong.");}

            //Lấy màu (Attribute 2)
            try
            {
            var element2 = browser.FindElement(By.CssSelector("[class=\"mc-ctclo\"]"));
            var color = element2.GetAttribute("innerHTML");
            product.Attribute2Value = Regex.Replace(color, "<i.*?</i>", "").Trim();
            }
            catch (Exception e)
            {Console.WriteLine(productLink + "\nColor went wrong.");}

            //Lấy giá hiện tại (Regular Price)
            try
            {
            var element3 = browser.FindElement(By.CssSelector("[class=\"mc-ctpri1\"]"));
            var price1 = element3.GetAttribute("innerHTML");
            price1 = Regex.Replace(price1, ".*?<span>", "").Trim();
            price1 = Regex.Replace(price1, "đ</span>.*", "", RegexOptions.Singleline).Trim();
            price1 = price1.Replace(".", "");
            product.RegularPrice = Double.Parse(price1); 
            }
            catch (Exception e)
            {Console.WriteLine(productLink + "\nPrice went wrong.");}

            //Lấy giá máy mới (Attribute 3)
            try
            {
            var element4 = browser.FindElement(By.CssSelector("[class=\"mc-ctpri2\"]"));
            product.Attribute3Value = element4.GetAttribute("innerHTML");
             }
            catch (Exception e)
            {Console.WriteLine(productLink + "\nNewPrice went wrong.");}

            //Tiết kiệm (Attribute 4)
            try
            {
            var element5 = browser.FindElement(By.CssSelector("[class=\"mc-ctpri3\"]"));
            var price3 = element5.GetAttribute("innerHTML");
            product.Attribute4Value = Regex.Replace(price3, "<p>|</p>", "").Trim();
             }
            catch (Exception e)
            {Console.WriteLine(productLink + "\nSavings went wrong.");}

            //Tình trạng (Attribute 5)
            try
            {
            var element6 = browser.FindElement(By.CssSelector("[class=\"mc-ctttm\"]"));
            string status = element6.GetAttribute("innerHTML");
            string stttemp= Regex.Match(status, "<li>(.*?)</li>", RegexOptions.Singleline).Groups[1].Value;
            product.Attribute5Value = stttemp;
            status = status.Replace(stttemp, "");

            //Phụ kiện (Attribute 6)
            stttemp = Regex.Match(status, "<li></li>.*?<li>(.*?)</li>", RegexOptions.Singleline).Groups[1].Value;
            product.Attribute6Value = stttemp;
            status = status.Replace(stttemp, "");

            //Bảo hành (Attribute 7)
            stttemp = Regex.Match(status, "<li></li>.*?<li></li>.*?<li>(.*?)</li>", RegexOptions.Singleline).Groups[1].Value;
            product.Attribute7Value = stttemp;
            }
            catch (Exception e)
            { Console.WriteLine(productLink + "\nStatus, Acess & Grua went wrong."); }

            //Thông số kĩ thuật (Description)
            try
            {
            var element7 = browser.FindElement(By.CssSelector("[class=\"modal-body tskt-popct\"]"));
            string infor = element7.GetAttribute("innerHTML");
            infor.Trim();
            infor = infor.Replace("@", "&");
            product.Description = infor;
             }
            catch (Exception e)
            {Console.WriteLine(productLink + "\nDecript went wrong.");}

            //Hình ảnh (Images)
            try
            {
            var element8 = browser.FindElement(By.CssSelector("[class=\"slick-list draggable\"]"));
            string img = element8.GetAttribute("innerHTML");
            string imgLinks ="";
            string pattern = "https://.*?\"";
            Regex r = new Regex(pattern);
            foreach (Match m in r.Matches(img))
                {
                  imgLinks += m.Value + ", ";
                  //Hiển thị kết quả
                }
            //loại bỏ cdn. mới truy cập được
            imgLinks = imgLinks.Replace("cdn.", "").Replace("\"","");
            imgLinks = imgLinks.TrimEnd(',');
            product.Images = imgLinks;
            }
            catch (Exception e)
            {Console.WriteLine(productLink + "\nImage went wrong.");}

            //lấy từng địa chỉ cho từng biến (2 địa chỉ)
            //string imageLink1 = Regex.Match(img, "src=\"(.*?)\"", RegexOptions.Singleline).Groups[1].Value;
            //imageLink1 = imageLink1.Replace("cdn.", "");
            //img = img.Replace("src=\"" + imageLink1 + "\"", "");
            //string imageLink2 = Regex.Match(img, "src=\"(.*?)\"", RegexOptions.Singleline).Groups[1].Value;
            //imageLink2 = imageLink2.Replace("cdn.", "");

            //Lấy Địa chỉ cửa hàng (Địa chỉ -> Attribute8, link ->Attribute9)
            try
            {
            var element9 = browser.FindElement(By.CssSelector("[class=\"mc-ctlocit\"]"));
            string location= element9.GetAttribute("innerHTML");
            var address = Regex.Match(location, "(.*?)<a", RegexOptions.Singleline).Groups[1].Value;
            product.Attribute8Value = address;
            string locLink = Regex.Match(location, "href=\"(.*?)\"", RegexOptions.Singleline).Groups[1].Value;
            product.Attribute9Value = locLink;
            }
            catch (Exception e)
            {Console.WriteLine(productLink + "\n Address went wrong.");}


           /* //Select elements by CSS Selector (easiest way)
            //You can also select element by ID, Class, Name, XPath,...
            //Get brand by CSS Attribute Selectors (https://www.w3schools.com/css/css_attribute_selectors.asp)
            var element = browser.FindElement(By.CssSelector("[itemprop=\"brand manufacturer\"]>a"));
            product.Attribute1Value = element.GetAttribute("innerHTML"); //OuterHTML will give full element HTML code

            //Get price by CSS Selectors (https://www.w3schools.com/css/css_selectors.asp)
            element = browser.FindElement(By.CssSelector("#final-price")); //No timeout, wait until page loaded
            string finalPrice = element.GetAttribute("innerHTML");
            finalPrice = finalPrice.Replace("$", "");
            double finalPriceInVnd = Double.Parse(finalPrice) * 24300 * 1.1 + 350000;
            product.SalePrice = finalPriceInVnd;
            */
       
            //----------------------------------------------

            return product;
        }
    }
}