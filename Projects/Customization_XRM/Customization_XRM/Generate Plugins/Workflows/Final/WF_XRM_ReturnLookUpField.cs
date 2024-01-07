using System;
using System.Activities;
using System.Linq;

// namespace for d365 
using Customization_XRM.Base.Common_URL;
using Customization_XRM.Base.WorkFlowBase;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace Customization_XRM.Generate_Plugins.Workflows.Final
{
    public class WF_XRM_ReturnLookUpField : WorkflowBase
    {
        public WF_XRM_ReturnLookUpField() : base(typeof(WF_XRM_ReturnLookUpField))
        {

        }

        /// <summary>
        /// تعریف ورودی های گردش کار
        /// </summary>

        //که به طور پیش فرض روی همه فرم ها ایجاد می شود { Record URL(Dynamic)(...)} مقدار فیلد 
        [RequiredArgument]
        [Input("Record URL")]
        [ReferenceTarget("")]
        public InArgument<String> RecordURL { get; set; }

        // LookUp نام اولین فیلد
        [Input("First LookUp Field Name")]
        public InArgument<string> FirstLookupFiled { get; set; }

        // LookUp نام دومین فیلد
        [Input("Secound LookUp Field Name")]
        public InArgument<string> SecoundLookupField { get; set; }

        // نام فیلدی که از موجودیت در دو سطح پایین تر مورد نیاز می باشد.
        [Input("Target Field Name")]
        public InArgument<string> TargetField { get; set; }

        protected override void ExecuteWorkFlowLogic(LocalWorkflowExecution localWorkflowExecution)
        {
            //مقدار نداشته باشد، یعنی اتصال صورت نگرفته و خطا رخ داده است localWorkflowExecution در صورتی که 
            if (localWorkflowExecution == null)
            {
                throw new InvalidPluginExecutionException("Local Plugin Execution is not initialized correctly.");
            }

            /// <summary>
            ///  initialize plugin basic components
            /// </summary>  

            // Obtain the tracing service
            ITracingService tracingService = localWorkflowExecution.tracingService;

            // Obtain the execution context from the service provider.  
            IWorkflowContext context = localWorkflowExecution.pluginContext;

            // Obtain the IOrganizationService instance which you will need for web service calls.  
            IOrganizationService crmService = localWorkflowExecution.orgService;

            /// <summary>
            /// URL ایجاد شیء از کلاس مربوط به خواندن
            /// </summary>  
            Common objCommon = new Common(localWorkflowExecution.executeContext);
            objCommon.tracingService.Trace("Load CRM Service from context --- OK");

            // RecordURL خواندن مقدار فیلد ورودی 
            String _recordURL = this.RecordURL.Get(localWorkflowExecution.executeContext);

            // خالی باشد از برنامه خارج می شود RecordURL اگر مقدار ورودی  
            if (_recordURL == null || _recordURL == "")
            {
                return;
            }

            /// <summary>
            /// URL خواندن جز به جزء
            /// </summary>  
            string[] urlParts = _recordURL.Split("?".ToArray());
            string[] urlParams = urlParts[1].Split("&".ToCharArray());
            string ParentObjectTypeCode = urlParams[0].Replace("etc=", "");
            string entityName = objCommon.sGetEntityNameFromCode(ParentObjectTypeCode, objCommon.service);
            string ParentId = urlParams[1].Replace("id=", "");
            objCommon.tracingService.Trace("ParentObjectTypeCode=" + ParentObjectTypeCode + "--ParentId=" + ParentId);

            // GUID به URL تبدیل آیدی فرم پدر واکشی شده از
            Guid entityID = new Guid(ParentId);

            // در فرم اول LookUp خواندن مقدار فیلد
            Entity entity = crmService.Retrieve(entityName, entityID, new ColumnSet(FirstLookupFiled.Get(localWorkflowExecution.executeContext)));

            //خواندن مقدار لوک آپ اولین فیلد 
            var FLookUp = (EntityReference)(entity.Attributes[FirstLookupFiled.Get(localWorkflowExecution.executeContext)]);
            var RetriveFLookUp = crmService.Retrieve(FLookUp.LogicalName, FLookUp.Id, new ColumnSet(SecoundLookupField.Get(localWorkflowExecution.executeContext)));

            //خواندن مقدار لوک آپ دومین فیلد 
            var SLookUp = (EntityReference)(RetriveFLookUp.Attributes[SecoundLookupField.Get(localWorkflowExecution.executeContext)]);
            var RetriveSLookUp = crmService.Retrieve(SLookUp.LogicalName, SLookUp.Id, new ColumnSet(TargetField.Get(localWorkflowExecution.executeContext)));

            // خواندن نوع فیلد در فرم در دو سطح پایین تر
            var TField_Type = RetriveSLookUp.Attributes[TargetField.Get(localWorkflowExecution.executeContext)].GetType();
            var Type = TField_Type.Name;

            // آن را در خروجی به گردش کار پاس دهد Value بود، آنگاه مقدار  OptionSet بررسی شرط که در صورتی که نوع فیلد از نوع
            if (Type == "OptionSetValue")
            {
                var TField = RetriveSLookUp.GetAttributeValue<OptionSetValue>(TargetField.Get(localWorkflowExecution.executeContext)).Value;
                OutputTargetFieldOptionSet.Set(localWorkflowExecution.executeContext, TField);
            }
            // درغیر این صورت مقدار رشته را در خروجی پاس می دهد
            else
            {
                var TFeild1 = (EntityReference)(RetriveSLookUp.Attributes[TargetField.Get(localWorkflowExecution.executeContext)]);
                var RetriveTFeild = crmService.Retrieve(TFeild1.LogicalName, TFeild1.Id, new ColumnSet("name"));
                var nameTargetField = RetriveTFeild["name"].ToString();

                OutputTargetFieldLookUp.Set(localWorkflowExecution.executeContext, nameTargetField);
            }
        }

        // OptionSet برای فیلد از نوع Int پارامتر خروجی از نوع
        [Output("OutputFieldOptionSetValue")]
        public OutArgument<int> OutputTargetFieldOptionSet { get; set; }

        // نمی باشند OptionSet پارامتر خروجی برای فیلدهایی که از نوع
        [Output("OutputFieldLookUpName")]
        public OutArgument<string> OutputTargetFieldLookUp { get; set; }
    }
}