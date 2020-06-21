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
using System.Text;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace WebAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            //Prepare list of product links
            List<string> productLinks = new List<string>();

            /* 
            Insert thủ công các link chính
            Điện thoại: https://fptshop.com.vn/may-doi-tra/dien-thoai-cu-gia-re
            Laptop: https://fptshop.com.vn/may-doi-tra/may-tinh-xach-tay-cu-gia-re
            Máy tính bảng: https://fptshop.com.vn/may-doi-tra/may-tinh-bang-cu-gia-re
            Smartwatch: https://fptshop.com.vn/may-doi-tra/smartwatch-cu-gia-re
            */

            //Vào link chính
            IWebDriver browserChrome = new ChromeDriver();
            browserChrome.Navigate().GoToUrl("https://fptshop.com.vn/may-doi-tra/dien-thoai-cu-gia-re");


            //Xử lý button "xem thêm"
            var MoreButton = browserChrome.FindElement(By.CssSelector("[class=\"mc-lpmore\"]:last-of-type"));

            var Limit = MoreButton.GetAttribute("innerHTML"); //Lấy số lượng sản phẩm trên nút xem thêm
            Limit = Regex.Replace(Limit, ".*?Xem thêm ", "").Trim();
            Limit = Regex.Replace(Limit, " sản phẩm.*", "").Trim();
            int Qtt = int.Parse(Limit);

            for(int i=0; i<Qtt; i+=15) //FPT chỉ thể hiện 15 sp trên trang
            {
                MoreButton.Click();
                System.Threading.Thread.Sleep(5 * 1000); //Delay 5s mỗi lần nhấn để tải trang
                MoreButton = browserChrome.FindElement(By.CssSelector("[class=\"mc-lpmore\"]:last-of-type"));
            }


            //Lấy link vào list của mỗi loại
            var elementURL = browserChrome.FindElement(By.CssSelector("[class=\"mc-lpitem kmt-list-prod\"]"));
            string ListBrand = elementURL.GetAttribute("innerHTML");
            string pattern2 = "<h3.*?href=\".*?\""; //<h3 để xác định 1 trong 2 link trùng nhau trong cssSelector
            Regex r2 = new Regex(pattern2);
            foreach (Match m2 in r2.Matches(ListBrand))
            {
                string LinkToList = m2.Value;
                LinkToList = Regex.Replace(LinkToList,"<h3.*?href=", "").Replace("\"", "");
                LinkToList = "https://fptshop.com.vn" + LinkToList;
                browserChrome.Navigate().GoToUrl(LinkToList);
                
                var elementLinkProduct = browserChrome.FindElement(By.CssSelector("[class=\"mc-lprow clearfix\"]"));
                string ListProduct = elementLinkProduct.GetAttribute("innerHTML");
                
                //thiếu vòng lặp chỉ lấy được tối đa 15 link của mỗi list
                string pattern3 = "<h3.*?href=\".*?\"";
                Regex r3 = new Regex(pattern3);
                foreach (Match m3 in r3.Matches(ListProduct))
                {
                    string LinkToProduct = m3.Value;
                    LinkToProduct = Regex.Replace(LinkToProduct, "<h3.*?href=", "").Replace("\"", "");
                    LinkToProduct = "https://fptshop.com.vn" + LinkToProduct;
                    productLinks.Add(LinkToProduct);
                 }
            }

            //Test demo
            //productLinks.Add("https://fptshop.com.vn/may-doi-tra/dien-thoai-cu-gia-re/iphone-xs-max-512gb/2709818-htm");
            //productLinks.Add("https://fptshop.com.vn/may-doi-tra/dien-thoai-cu-gia-re/vsmart-joy-1-2gb-16gb/1798757-htm");
            

            //Write all Link for backup manual
            System.IO.StreamWriter writerlinks = new System.IO.StreamWriter("D:\\LinksFile.txt");
            foreach (string item in productLinks)
            {
                writerlinks.WriteLine(item);
            }
            writerlinks.Close();

            browserChrome.Close();

            //Run Jomashop crawler
            Crawler crawler = new Crawler();
            List<Models.Product> listProduct = crawler.Run(productLinks);

            //Generate WooCommerce Product Import template
            WooCommerceGenerator generator = new WooCommerceGenerator();
            generator.GenerateProductImportTemplate(listProduct, "D:\\shops22.csv");
        } 
    }
}


