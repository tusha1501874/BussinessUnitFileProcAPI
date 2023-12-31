﻿using BussinessUnitFileProcAPI.Entities;
using BussinessUnitFileProcAPI.Models;
using BussinessUnitFileProcAPI.Services;
using BussinessUnitFileProcAPI.Validators;
using Microsoft.AspNetCore.Mvc;

namespace BussinessUnitFileProcAPI.Controllers;
    [ApiController]
    [Route("[controller]")]
    public class BussinessUnitController : ControllerBase
    {
        private readonly ITableStorageService _storageService;
        public BussinessUnitController(ITableStorageService storageService)
        {
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        [HttpGet]
        [ActionName(nameof(GetAsync))]
        public async Task<IActionResult> GetAsync([FromQuery] string id)
        {
            List<BussinessUnitEntity>? result = await _storageService.GetEntityAsync(id);
            
            if (result!=null && result.Count>0)
                return Ok(result);
            else
            {
                Error errorMsg = new Error("RecordNotFound", "Record Not found For BatchId");
                return NotFound(errorMsg);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] BussinessUnitEntity entity)
        {

            entity.PartitionKey = entity.BusinessUnit;
            string Id = Guid.NewGuid().ToString();
            entity.BatchID = Id;
            entity.RowKey = Id;

            BussinessUnitValidator validator = new BussinessUnitValidator();
            FluentValidation.Results.ValidationResult ValidationResult = await validator.ValidateAsync(entity);

            if (ValidationResult.IsValid)
            {
                string result = await _storageService.InsertEntityAsync(entity);
                if (!String.IsNullOrEmpty(result))
                    return Ok($"BatchId: {result}");
                else return BadRequest(new Error("BatchNotCreated", "Batch not created please check logs"));
            }

            List<Error> errorMsg = ValidationResult.Errors.Select(x => new Error(x.ErrorCode, x.ErrorMessage)).ToList();
            return new BadRequestObjectResult(errorMsg);
        }
    }