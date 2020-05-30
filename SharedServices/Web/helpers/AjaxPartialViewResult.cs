using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SharedServices.Web.helpers
{
    public class AjaxPartialViewResult : ActionResult
    {
        private ActionResult InnerAction { get; set; }
        public override void ExecuteResult(ControllerContext context) { InnerAction.ExecuteResult(context); }
        public static AjaxPartialViewResult ToAjaxPartialViewResult(ActionResult action) => new AjaxPartialViewResult() { InnerAction = action };

    }
}
