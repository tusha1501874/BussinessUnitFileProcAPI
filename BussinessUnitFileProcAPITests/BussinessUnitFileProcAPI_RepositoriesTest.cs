using BussinessUnitFileProcAPI.Entities;
using BussinessUnitFileProcAPI.Models;
using BussinessUnitFileProcAPI.Repositories;
using BussinessUnitFileProcAPITests.Models;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System.Reflection;
using File = BussinessUnitFileProcAPI.Models.File;

namespace BussinessUnitFileProcAPITests;
    public class BussinessUnitFileProcAPI_RepositoriesTest
    {
        private IConfiguration _configuration;
        private ILogger<TableStorageRepo> _logger;
        private TableStorageRepo _tableStorageRepo;
        private CloudTableMock _cloudTable;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<TableStorageRepo>>();
            _configuration = A.Fake<IConfiguration>();
            _cloudTable = A.Fake<CloudTableMock>();
            _tableStorageRepo = new TableStorageRepo(_configuration, _logger, _cloudTable);
        }

        [TestCaseSource(nameof(TestCasesDataForGetAsyncTest))]
        public void GetEntityAsyncTest(string batchId, List<BussinessUnitEntity> bussinessUnitEntitiesFromTableStorage)
        {
            var ctor = typeof(TableQuerySegment<BussinessUnitEntity>)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(c => c.GetParameters().Count() == 1);
            var mockQuerySegment = ctor.Invoke(new object[] { bussinessUnitEntitiesFromTableStorage }) as TableQuerySegment<BussinessUnitEntity>;
            MethodInfo setTokenMethod = typeof(TableQuerySegment<BussinessUnitEntity>).GetMethod("set_ContinuationToken", BindingFlags.NonPublic | BindingFlags.Instance);
            var continuationToken = new TableContinuationToken();
            setTokenMethod.Invoke(mockQuerySegment, new object[] { continuationToken });
            A.CallTo(() => _cloudTable.ExecuteQuerySegmentedAsync(A<TableQuery<BussinessUnitEntity>>.Ignored, null)).Returns(Task.FromResult(mockQuerySegment));

            var result = _tableStorageRepo.GetEntityAsync(batchId);

            Assert.NotNull(result.Result);
        }

        [TestCaseSource(nameof(TestCasesDataForInsertEntityAsync))]
        public void InsertEntityAsyncTest(BussinessUnitEntity bussinessUnitEntity)
        {
            bussinessUnitEntity.BatchID = Guid.NewGuid().ToString();
            var generatedTableResultFromAzure = new TableResult() { Result = bussinessUnitEntity, HttpStatusCode = 200 };

            A.CallTo(() => _cloudTable.ExecuteAsync(A<TableOperation>.Ignored)).Returns(generatedTableResultFromAzure);

            var result = _tableStorageRepo.InsertEntityAsync(bussinessUnitEntity);

            Assert.NotNull(result.Result);
        }

        private static readonly object[] TestCasesDataForGetAsyncTest =
        {
           new object[]{
            "b8c01a7a-f863-465e-a34d-f633a520e879",
           new List<BussinessUnitEntity>(){
               new BussinessUnitEntity
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
        } } };

        private static readonly object[] TestCasesDataForInsertEntityAsync =
        {
           new object[]{
               new BussinessUnitEntity
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
        } };
    }
