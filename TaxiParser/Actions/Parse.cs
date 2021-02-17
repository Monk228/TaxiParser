using System;
using AngleSharp;
using OpenQA.Selenium.Chrome;
using TaxiParser.Models;
using System.Threading;
using TaxiParser.Services;
using System.Collections.Generic;
using AngleSharp.Dom;

namespace TaxiParser
{
    class Parse
    {
        private static readonly IConfiguration Config = Configuration.Default;
        private static readonly IBrowsingContext Context = BrowsingContext.New(Config);

        private static Helpers _Helpers = new Helpers(Context, Config);

        static ChromeDriver _driver;

      
        public static void Start()
        {
            long pageCounter = 1;
            long maxPage = 0;
            string url = "https://services.govvrn.ru/wps/taxi/zul/permitDocs.zul";

            var chromeOptions = new ChromeOptions();
           
            var result = new List<ParsedTaxi>();

            _driver = new ChromeDriver(chromeOptions);
            _driver.Manage().Window.Maximize();

            Console.WriteLine("Go to url...");

            try
            {
                _driver.Navigate().GoToUrl(url);
                Thread.Sleep(500);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            
            try
            {
                var pageCount = _Helpers.TakeElements(_driver.PageSource, m => m.ClassName == "z-paging-text");

                maxPage = Int64.Parse(pageCount[1].TextContent.Substring(1));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            Console.WriteLine("Count of pages = " + maxPage);

            

            do
            {
                Console.WriteLine("Start parsing...");

                result.AddRange(ParsePage(_driver.PageSource));

                Thread.Sleep(1000);

                Console.WriteLine(pageCounter + " page was parsed successfully");

                if (result == null)
                {   
                    Console.WriteLine("RESULT IS NULL");
                    return;
                }

                    pageCounter++;

                var btn = _driver.FindElementByClassName("z-paging-next");
                if (btn != null)
                {
                    Console.WriteLine("Going to next page...");
                    btn.Click();
                    Thread.Sleep(2000);
                }
                

            } while (pageCounter != maxPage);

            Console.WriteLine("Saving result...");
            Save(result);

            Thread.Sleep(120000);
            Start();

        }
        private static void Save(List<ParsedTaxi> result)
        {
            //TODO : to database
        }
        public static List<ParsedTaxi> ParsePage(string html)
        {
            var result = new List<ParsedTaxi>();
            List<IElement> table = new List<IElement>();
            try
            {
                table = _Helpers.TakeElements(html, m => m.ClassName == "z-rows");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }     
           
            if (table.Count != 1)
            {
                Console.WriteLine("Cant find table");
                return null;
            }

            var rows = table[0].Children;
            if (rows.Length <= 0)
            {
                Console.WriteLine("Found nothing in table");
                return null;
            }

            foreach (var row in rows)
            {
                
                var children = row.Children;
                if (children.Length <= 0)
                {
                    Console.WriteLine("Found nothing in rows");
                    return null;
                }

                result.Add(ParseChildren(children));

            }

            if (result != null)
            {
                return result;
            }
            return null;
        }
        public static ParsedTaxi ParseChildren(IHtmlCollection<IElement> children)
        {
            var taxi = new ParsedTaxi();
            
                for (int i = 0; i < children.Length; i++)
                {

                    var data = children[i].TextContent;

                    switch (i)
                    {
                        case 0:
                            taxi.Status = data;
                            break;
                        case 1:
                            taxi.PermissionNumber = data;
                            break;

                        case 2:
                            taxi.RegisterNumber = data;
                            break;

                        case 3:
                            taxi.OrderDate = DateTime.Parse(data);
                            break;

                        case 4:
                            taxi.OrderNumber = data;
                            break;

                        case 5:
                            taxi.EndTime = DateTime.Parse(data);
                            break;

                        case 6:
                            taxi.LicensePlate = data;
                            break;

                        case 7:
                            string[] car = data.Split(" ");
                            for (int z = 0; z < car.Length; z++)
                            {
                                if (z == 0)
                                {
                                    taxi.CarBrand = car[z];
                                    continue;
                                }
                                taxi.CarModel += " " + car[z];
                            }
                            break;

                        case 8:
                            taxi.ReleaseYear = Int32.Parse(data);
                            break;

                        case 9:
                            long num = Int64.Parse(data);
                            int countOfNumbers = data.Length;
                            if (countOfNumbers == 13)
                            {
                                taxi.OGRN = num;
                                break;
                            }
                            taxi.OGRNIP = num;
                            break;

                        case 10:
                            if (data.Contains("ИП"))
                            {
                                data.Substring(3);
                                taxi.IPFullName = data;
                                break;
                            }

                            taxi.CompanyName = data;
                            break;
                        case 11:

                            string[] adress = data.Split("тел");
                            taxi.Adress = adress[0];

                            string[] phone = adress[1].Split("/");
                            taxi.PhoneNumber = phone[0];

                            taxi.ContactDetails = data;
                            break;
                    }
                }
       
            return taxi;
        }


    }
}
