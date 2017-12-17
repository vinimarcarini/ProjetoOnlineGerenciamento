using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GerenciamentoOnline.Models
{

    [DebuggerDisplayAttribute("{Id}-{FirstName}")]
    public class Person
    {
        [Display(Order = 1)] //<--- set custom title
        public string Descricao { get; set; }

        [Display(Order = 2)]
        public string TotalPorcentagem { get; set; }

        [Display(Order = 0)] //<--- specify order
        public int Id { get; set; }
       

        //[Display(AutoGenerateField = false)]
        //public DateTime BirthDate { get; set; }

        //public string DateOfBirth { get { return BirthDate.ToShortDateString(); } } //<--- title split camel-case
        //public string Location { get; set; }

        [HiddenInput(DisplayValue = false)] //<--- ignore field (method 1)
        public string HiddenField1 { get; set; }

        [Display(AutoGenerateField = false)] //<--- ignore field (method 2)
        public string HiddenField2 { get; set; }
    }
}