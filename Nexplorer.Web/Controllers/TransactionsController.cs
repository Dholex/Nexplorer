﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nexplorer.Config;
using Nexplorer.Data.Query;
using Nexplorer.Domain.Criteria;
using Nexplorer.Domain.Dtos;
using Nexplorer.Domain.Enums;
using Nexplorer.Domain.Models;
using Nexplorer.Web.Dtos;
using Nexplorer.Web.Models;

namespace Nexplorer.Web.Controllers
{
    public class TransactionsController : WebControllerBase
    {
        private readonly BlockQuery _blockQuery;
        private readonly TransactionQuery _transactionQuery;

        private const int MaxTxsPerFilterPage = 100;

        public TransactionsController(BlockQuery blockQuery, TransactionQuery transactionQuery)
        {
            _blockQuery = blockQuery;
            _transactionQuery = transactionQuery;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Transaction(string txHash)
        {
            if (txHash == null)
                return RedirectToAction("index");

            var tx = await _transactionQuery.GetTransaction(txHash);

            if (tx == null)
                return RedirectToAction("index");

            var blockHash = await _blockQuery.GetBlockHashAsync(tx.BlockHeight);

            var viewModel = new TransactionViewModel
            {
                Transaction = tx,
                BlockHash = blockHash
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GetTransactions(DataTablePostModel<TransactionFilterCriteria> model)
        {
            var count = model.Length > MaxTxsPerFilterPage
                ? MaxTxsPerFilterPage
                : model.Length;
            
            var data = await _transactionQuery.GetTransactionsFilteredAsync(model.FilterCriteria, model.Start, count, true, 1000);
            
            var response = new
            {
                RecordsFiltered = data.ResultCount,
                Data = data.Results
            };

            return Ok(response);
        }
    }
}
