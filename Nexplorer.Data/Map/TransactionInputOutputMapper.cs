﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nexplorer.Data.Context;
using Nexplorer.Domain.Dtos;
using Nexplorer.Domain.Entity.Blockchain;

namespace Nexplorer.Data.Map
{
    public class TransactionInputOutputMapper
    {
        private readonly Dictionary<string, Address> _addressCache;

        public TransactionInputOutputMapper()
        {
            _addressCache = new Dictionary<string, Address>();
        }

        public async Task<List<T>> MapTransactionInputOutput<T>(NexusDb nexusDb, IEnumerable<TransactionInputOutputDto> txInOutDtos, Block block, Transaction transaction) where T : TransactionInputOutput, new()
        {
            var txInOuts = new List<T>();

            foreach (var txInOutDto in txInOutDtos)
            {
                var cachedAddress = _addressCache.ContainsKey(txInOutDto.AddressHash);

                var address = (cachedAddress
                                  ? _addressCache[txInOutDto.AddressHash]
                                  : await nexusDb.Addresses
                                      .FirstOrDefaultAsync(x => x.Hash == txInOutDto.AddressHash)) ??
                              new Address { Hash = txInOutDto.AddressHash, FirstBlock = block };

                if (address.FirstBlock == null)
                    address.FirstBlock = block;

                if (!cachedAddress)
                    _addressCache.Add(address.Hash, address);

                txInOuts.Add(new T
                {
                    Transaction = transaction,
                    Amount = txInOutDto.Amount,
                    Address = address
                });
            }

            return txInOuts;
        }

        public void Reset()
        {
            _addressCache.Clear();
        }
    }
}