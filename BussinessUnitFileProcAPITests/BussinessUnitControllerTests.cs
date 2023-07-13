using BussinessUnitFileProcAPI.Controllers;
using BussinessUnitFileProcAPI.Entities;
using BussinessUnitFileProcAPI.Services;
using FakeItEasy;
using BussinessUnitFileProcAPI.Models;
using File = BussinessUnitFileProcAPI.Models.File;
using Microsoft.AspNetCore.Mvc;

namespace BussinessUnitFileProcAPITests;
    public class Tests
    {
        private ITableStorageService _storageService;
        private BussinessUnitController _bussinessUnitController;

        [SetUp]
        public void Setup()
        {
            _storageService = A.Fake<ITableStorageService>();
            _bussinessUnitController = new BussinessUnitController(_storageService);
        }

        [Test]
        [TestCaseSource(nameof(Generator))]
        public void PostAsyncTest(BussinessUnitEntity bussinessUnitEntity, int expectedResult, string testType)
        {
            A.CallTo(_storageService).Where(call => call.Method.Name == "InsertEntityAsync")
                .WithReturnType<Task<string>>()
                .Returns("XYZ");

            var result = _bussinessUnitController.PostAsync(bussinessUnitEntity);
            var statusCodeOfResult = result.Result as ObjectResult;
            var errorlList = statusCodeOfResult.Value as List<Error>;

            Assert.NotNull(statusCodeOfResult);
            Assert.That(statusCodeOfResult.StatusCode, Is.EqualTo(expectedResult));

            if (testType == "ValidInput")
                Assert.NotNull(statusCodeOfResult.Value);

            if (testType == "InvalidInputBussinessUnitEmptyTest")
                Assert.That(errorlList[0].Description, Is.EqualTo("Bussiness Unit Should not be null or empty."));

            if (testType == "InvalidInputForFileListAtlestContainOneItemTest")
                Assert.That(errorlList[0].Description, Is.EqualTo("⦁\tAtleast one file should be exists in batch request body data"));

            if (testType == "InvalidInputPastExpiryDateTest")
                Assert.That(errorlList[0].Description, Is.EqualTo("Expiry Date should not be a past date."));
        }

        [Test]
        [TestCase("b8c01a7a-f863-465e-a34d-f633a520e879", 200)]
        [TestCase("xyz", 404)]
        public void GetAsyncTest(string batchId, int expectedStatusCode)
        {
            A.CallTo(() =>
            _storageService.GetEntityAsync("b8c01a7a-f863-465e-a34d-f633a520e879")).Returns(new List<BussinessUnitEntity>() { new BussinessUnitEntity { BusinessUnit = "xyz" } });

            var result = _bussinessUnitController.GetAsync(batchId);
            var statusCodeOfResult = result.Result as ObjectResult;
            var errorlList = statusCodeOfResult.Value as List<Error>;
            Assert.NotNull(statusCodeOfResult);
            Assert.That(statusCodeOfResult.StatusCode, Is.EqualTo(expectedStatusCode));
        }

        public static IEnumerable<TestCaseData> Generator()
        {
            yield return new TestCaseData(new BussinessUnitEntity
            {
                BusinessUnit = "TestBU",
                acl = new Acl()
                {
                    ReadUsers = new List<ReadUser>()
                  {
                      new ReadUser()
                      {
                          Name="TestReadUser"
                      }
                  },
                    ReadGroups =
                  new List<ReadGroup>()
                  {
                      new ReadGroup()
                      {
                          Name="TestReadGroups"
                      }
                  }
                },
                Attributes = new List<Attributes>()
                {
                    new Attributes()
                    {
                       Keys="TestParentKey",
                       Values="TestParentValue"
                    }
                },
                expiryDate = DateTime.Now.Date,
                Files = new List<File>()
                {
                    new File()
                    {
                        Filename="TestFileName",
                        FileSize=1234,
                        MimeType="Json",
                        Hash="xyz",
                        Attributes= new List<Attributes>()
                        {
                             new Attributes()
                    {
                       Keys="TestFileKey",
                       Values="TestFileValue"
                    }
                        }

                    }
                }
            }
            , 200, "ValidInput").SetName("Valid Test");

            yield return new TestCaseData(new BussinessUnitEntity
            {
                BusinessUnit = "",
                acl = new Acl()
                {
                    ReadUsers = new List<ReadUser>()
                  {
                      new ReadUser()
                      {
                          Name="TestReadUser"
                      }
                  },
                    ReadGroups =
                  new List<ReadGroup>()
                  {
                      new ReadGroup()
                      {
                          Name="TestReadGroups"
                      }
                  }
                },
                Attributes = new List<Attributes>()
                {
                    new Attributes()
                    {
                       Keys="TestParentKey",
                       Values="TestParentValue"
                    }
                },
                expiryDate = DateTime.Now.Date,
                Files = new List<File>()
                {
                    new File()
                    {
                        Filename="TestFileName",
                        FileSize=1234,
                        MimeType="Json",
                        Hash="xyz",
                        Attributes= new List<Attributes>()
                        {
                             new Attributes()
                    {
                       Keys="TestFileKey",
                       Values="TestFileValue"
                    }
                        }

                    }
                }

            }, 400, "InvalidInputBussinessUnitEmptyTest").SetName("Invalid Test with Empty/null bussiness Unit");

            yield return new TestCaseData(new BussinessUnitEntity
            {
                BusinessUnit = "TestBU",
                acl = new Acl()
                {
                    ReadUsers = new List<ReadUser>()
                  {
                      new ReadUser()
                      {
                          Name="TestReadUser"
                      }
                  },
                    ReadGroups =
                new List<ReadGroup>()
                {
                      new ReadGroup()
                      {
                          Name="TestReadGroups"
                      }
                }
                },
                Attributes = new List<Attributes>()
                {
                    new Attributes()
                    {
                       Keys="TestParentKey",
                       Values="TestParentValue"
                    }
                },
                expiryDate = DateTime.Now.Date,
                Files = new List<File>()
                {

                }
            }
          , 400, "InvalidInputForFileListAtlestContainOneItemTest").SetName("Invalid Test For File At least contain one item in list");

            yield return new TestCaseData(new BussinessUnitEntity
            {
                BusinessUnit = "TestBU",
                acl = new Acl()
                {
                    ReadUsers = new List<ReadUser>()
                  {
                      new ReadUser()
                      {
                          Name="TestReadUser"
                      }
                  },
                    ReadGroups =
                  new List<ReadGroup>()
                  {
                      new ReadGroup()
                      {
                          Name="TestReadGroups"
                      }
                  }
                },
                Attributes = new List<Attributes>()
                {
                    new Attributes()
                    {
                       Keys="TestParentKey",
                       Values="TestParentValue"
                    }
                },
                expiryDate = DateTime.Today.AddDays(-5),
                Files = new List<File>()
                {
                    new File()
                    {
                        Filename="TestFileName",
                        FileSize=1234,
                        MimeType="Json",
                        Hash="xyz",
                        Attributes= new List<Attributes>()
                        {
                             new Attributes()
                    {
                       Keys="TestFileKey",
                       Values="TestFileValue"
                    }
                        }

                    }
                }
            }
            , 400, "InvalidInputPastExpiryDateTest").SetName("Invalid Test with Past ExpiryDate");
        }

    }