using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// namespace for d365 
using Customization_XRM.Base.Common_URL;
using Customization_XRM.Base.WorkFlowBase;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace Customization_XRM.Generate_Plugins.Workflows.Final
{
    public class WF_XRM_ChangeOptionSetDetailForm : WorkflowBase
    {
        public WF_XRM_ChangeOptionSetDetailForm() : base(typeof(WF_XRM_ChangeOptionSetDetailForm))
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

        // نام موجودیت فرزند
        [Input("Child Entity Name")]
        public InArgument<string> ChildEntityName { get; set; }

        // که قرار است تغییر کند OptionSet نام فیلد 
        [Input("OptionSet Field Name")]
        public InArgument<string> OptionSetFieldName { get; set; }

        // که قرار است تغییر کند OptionSet فیلد Value مقدار
        [Input("OptionSet Field Value")]
        public InArgument<int> OptionSetFieldValue { get; set; }

        // نام فیلد لوک آپ روی موجودیت فرزند
        [Input("LookUp Field Name")]
        public InArgument<string> LookUpFieldName { get; set; }

        protected override void ExecuteWorkFlowLogic(LocalWorkflowExecution localWorkflowExecution)
        {
            try
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

                // خواندن مقادیر و فیلدهای موجودیت پدر
                Entity entity = crmService.Retrieve(entityName, entityID, new ColumnSet(true));

                // واکشی مقادیر موجودیت های فرزند مرتبط با موجودیت پدر
                var entityItemsQuery = new QueryExpression
                {
                    EntityName = ChildEntityName.Get(localWorkflowExecution.executeContext),
                    ColumnSet = new ColumnSet(true),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = LookUpFieldName.Get(localWorkflowExecution.executeContext),
                                Operator = ConditionOperator.Equal,
                                Values = { entity.Id}
                            }
                        }
                    }
                };

                // تبدیل به لیست جهت پیمایش آن
                var entityItems = crmService.RetrieveMultiple(entityItemsQuery).Entities.ToList();

                // روی موجودیت فرزند OptionSet به روزرسانی مقدار فیلد  
                foreach (var item in entityItems)
                {
                    // خواندن مقادیر و فیلدهای موجودیت فرزند
                    Entity entityToUpdate = new Entity(item.LogicalName, item.Id);

                    // روی موجودیت فرزند OptionSet وارد شده در ورودی جهت به روزرسانی فیلد  Value خواندن مقدار 
                    OptionSetValue OptionVal = new OptionSetValue(OptionSetFieldValue.Get(localWorkflowExecution.executeContext));

                    //شده است Set حذف مقدار قبلی که
                    entityToUpdate.Attributes.Remove(OptionSetFieldName.Get(localWorkflowExecution.executeContext));
                    // موجودیت فرزند OptionSet ایجاد مقدار جدید برای فیلد 
                    entityToUpdate.Attributes.Add(OptionSetFieldName.Get(localWorkflowExecution.executeContext), OptionVal);

                    // به روزرسانی موجودیت های فرزند
                    crmService.Update(entityToUpdate);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}