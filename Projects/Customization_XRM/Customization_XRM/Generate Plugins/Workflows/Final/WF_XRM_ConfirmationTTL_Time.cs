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
    public class WF_XRM_ConfirmationTTL_Time : WorkflowBase
    {
        public WF_XRM_ConfirmationTTL_Time() : base(typeof(WF_XRM_ConfirmationTTL_Time))
        {

        }

        /********************************************************/
        /// <summary>
        /// تعریف ورودی های گردش کار
        /// </summary>

        //1)  که به طور پیش فرض روی همه فرم ها ایجاد می شود { Record URL(Dynamic)(...)} مقدار فیلد  
        [RequiredArgument]
        [Input("Record URL")]
        [ReferenceTarget("")]
        public InArgument<String> RecordURL { get; set; }

        //2)  نام فیلد تأیید
        [Input("Confirmed Field Name")]
        public InArgument<string> ConfirmName { get; set; }

        //3)  نام فیلد مجموع زمان تأییدات
        [Input("Sum Date Confirm Field")]
        public InArgument<string> SumDate { get; set; }

        //4)  نام فیلد تاریخ تأیید
        [Input("Confirm Date Field Name")]
        public InArgument<string> ConfirmDate { get; set; }

        /********************************************************/
        /// <summary>
        /// تعریف متغیرهای مورد نیاز پلاگین
        /// </summary>

        //1)  اتفیلد تأیید Value مقدار
        int optionSetValue;

        //2)  مقدار محاسبه شده مجموع زمان تأییدات
        int confirmSumDate;

        //3)  بررسی داشتن یا نداشتن مقدار فیلد آخرین زمان تأییدات
        bool dateBool;

        //4)  مقدار زمان تأییدات
        DateTime confirmDate;

        //5)  (CreateOn) مقدار زمان ایجاد رکورد 
        DateTime createOn;

        //6)  مقدار آخرین زمان تأیید
        DateTime lastConfirmDate;

        /********************************************************/
        /// <summary>
        /// Dynamics 365  کردن کلاس اتصال ب override 
        /// </summary>
        /// <param name="localWorkflowExecution"></param>
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

                // خواندن مقادیر ستون کد از فرم پدر
                Entity entity = crmService.Retrieve(entityName, entityID, new ColumnSet(true));

                //  کوئری خواندن مقادیر رکورد جار
                QueryExpression confirmQuery = new QueryExpression
                {
                    EntityName = entityName,
                    ColumnSet = new ColumnSet(true),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = entityName.ToLower() + "id",
                                Operator = ConditionOperator.Equal,
                                Values = {entity.Id}
                            }
                        }
                    }
                };

                // خواندن مقادیر از کوئری
                DataCollection<Entity> confirmItemDetails = crmService.RetrieveMultiple(confirmQuery).Entities;

                /// <summary>
                ///  مقداردهی فیلدهای زیر:
                ///  1-  تاریخ تأیید
                ///  2-  زمان ایجاد
                ///  3-  آخرین تاریخ تأیید
                /// </summary>  
                foreach (var confirm in confirmItemDetails)
                {
                    confirmDate = confirm.GetAttributeValue<DateTime>(ConfirmDate.Get(localWorkflowExecution.executeContext));
                    createOn = confirm.GetAttributeValue<DateTime>("createdon");

                    // بررسی مقدار داشتن یا نداشتن فیلد آخرین تاریخ تأیید
                    // تعریف شود new_last_confirmed_date فیلد آخرین تاریخ تأیید در کلیه سامانه ها باید 
                    dateBool = confirm.Contains("new_last_confirmed_date");

                    if (dateBool == true)
                    {
                        lastConfirmDate = confirm.GetAttributeValue<DateTime>("new_last_confirmed_date");
                    }
                }

                /*بررسی شرط خالی بودن مقادیر تأیید*/
                if (entity.GetAttributeValue<OptionSetValue>(ConfirmName.Get(localWorkflowExecution.executeContext)) is null)
                {
                    return;
                }

                /// <summary>
                /// مقادیر مجموع زمان تأییدات و تأییدات
                /// </summary>
                /* فیلد تأییدات و مقدار فیلد مجموع زمان تأییدات value خواندن مقدار*/
                if (entity.GetAttributeValue<OptionSetValue>(ConfirmName.Get(localWorkflowExecution.executeContext)) != null)
                {
                    // مقدار فیلد تأییدات
                    optionSetValue = Convert.ToInt32(entity.GetAttributeValue<OptionSetValue>(ConfirmName.Get(localWorkflowExecution.executeContext)).Value);

                    // خواندن مقدار فیلد مجموع زمان
                    confirmSumDate = entity.GetAttributeValue<int>(SumDate.Get(localWorkflowExecution.executeContext));
                }

                /// <summary>
                /// بررسی شروط تأییدات
                /// </summary>               
                //  اگر فیلد تأیید تغییر کرده باشد
                if (optionSetValue != 0)
                {
                    // اگر فیلد آخرین زمان تأییدات خالی از مقدار بود
                    if (dateBool == false)
                    {
                        // محاسبه اختلاف زمان تأیید از زمان ایجاد و جمع آن با مقدار مجموع زمان تأیید
                        int saleDiff = Convert.ToInt32((confirmDate - createOn).TotalMinutes);
                        int result = (saleDiff + confirmSumDate);

                        // اضافه کردن مجموع محاسبه شده در فیلد مجموع زمان تأییدات در سامانه
                        entity.Attributes.Add(SumDate.Get(localWorkflowExecution.executeContext), result);
                        crmService.Update(entity);
                    }
                    else
                    {
                        // محاسبه اختلاف زمان تأیید از زمان ایجاد و جمع آن با مقدار مجموع زمان تأیید
                        int saleDiff = Convert.ToInt32((confirmDate - lastConfirmDate).TotalMinutes);
                        int result = (saleDiff + confirmSumDate);

                        // به روزرسانی فیلد مجموع زمان تأییدات در سامانه
                        entity.Attributes[SumDate.Get(localWorkflowExecution.executeContext)] = result;
                        crmService.Update(entity);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}