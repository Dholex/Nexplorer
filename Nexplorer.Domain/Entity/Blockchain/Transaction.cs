﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProtoBuf;

namespace Nexplorer.Domain.Entity.Blockchain
{
    [Table("Transaction")]
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }
        
        [Required]
        public int BlockHeight { get; set; }

        [ForeignKey("BlockHeight")]
        public Block Block { get; set; }
        
        [Required]
        [MaxLength(256)]
        public string Hash { get; set; }

        [Required]
        public int Confirmations { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public double Amount { get; set; }
        
        public List<TransactionInput> Inputs { get; set; }
        public List<TransactionOutput> Outputs { get; set; }
    }
}
