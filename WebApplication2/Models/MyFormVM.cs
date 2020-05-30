using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class MySelectListItem { 
        public string Display { get; set; }
        public int Number { get; set; }
    }
    public class MyNestedItem
    {
        [Display(Name = "My Nested Item")]
        public string Text { get; set; }
    }
    public class MyFormVM
    {
        public IEnumerable<MySelectListItem> MySelections { get; private set; }
        [Required(ErrorMessage = "Field is Required")]
        [MaxLength(20, ErrorMessage = "maximum {1} characters allowed")]
        [MinLength(10, ErrorMessage = "minimum {1} characters")]
        //[StringLength(10, ErrorMessage = "maximum {1} characters allowed")]
        [Display(Name="My Text")]
        public string MyTextInput { get; set; }

        //[Required(ErrorMessage = "Field is Required")]
        [Display(Name = "My Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? MyDate { get; set; }

        [Range(0.01, 100.00,
            ErrorMessage = "Price must be between 0.01 and 100.00")]
        [Display(Name = "My Price")]
        public decimal Price { get; set; }

        //[Required(ErrorMessage = "Field is Required")]
        [Display(Name = "My Select")]
        public int? MySelect { get; set; }
        [Display(Name = "My Checkbox")]
        public bool MyCheckbox { get; set; }

        public MyNestedItem MyNestedItem { get; set; }
        public MyFormVM() {
            MySelections = new MySelectListItem[5]
            {
                new MySelectListItem{ Number=100, Display="One Hundred" },
                new MySelectListItem{ Number=200, Display="Two Hundred" },
                new MySelectListItem{ Number=300, Display="Three Hundred" },
                new MySelectListItem{ Number=400, Display="Four Hundred" },
                new MySelectListItem{ Number=500, Display="Five Hundred" }
                
            };
            MySelect = 200;
        }
    }
}