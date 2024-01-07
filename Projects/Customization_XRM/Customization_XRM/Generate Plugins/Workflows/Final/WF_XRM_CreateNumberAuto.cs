using System;
using System.Activities;
using System.Linq;

// namespace for d365 
using Customization_XRM.Base.Common_URL;
using Customization_XRM.Base.WorkFlowBase;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace Customization_XRM.Generate_Plugins
{
    public class WF_XRM_CreateNumberAuto : WorkflowBase
    {
        public WF_XRM_CreateNumberAuto() : base(typeof(WF_XRM_CreateNumberAuto))
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

        //ورودی برای گرفتن آیدی فرم تنظیمات
        [Input("Setting ID")]
        //[ReferenceTarget("new_settings")]
        [ReferenceTarget("new_systemcirculationsettings")]
        public InArgument<EntityReference> SettingID { get; set; }

        //ورودی برای گرفتن نام فیلدی که قرار است کد روی آن ایجاد شود
        [Input("Attribute Name")]
        public InArgument<string> AttributeName { get; set; }

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
                Entity entity = crmService.Retrieve(entityName, entityID, new ColumnSet(AttributeName.Get(localWorkflowExecution.executeContext)));

                /// <summary>    
                /// خواندن مقادیر فیلدهای زیر از فرم تنظیمات:
                /// پیشوند (new_pre_code)
                /// طول پسوند کد (new_length_code)
                /// شمارنده (new_code)
                /// جداکننده (new_separator)
                /// </summary>
                Entity settings = crmService.Retrieve("new_systemcirculationsettings", SettingID.Get(localWorkflowExecution.executeContext).Id,
                                                              new ColumnSet("new_prefixcode", "new_lengthsuffixcode", "new_countercode", "new_separatorcode", "new_lastcode"));

                //Get Value From Settings
                var preCode = settings.Attributes["new_prefixcode"];
                var lengthCode = settings.Attributes["new_lengthsuffixcode"];
                var code = settings.Attributes["new_countercode"];
                var separator = settings.Attributes["new_separatorcode"];

                Object maxCode;

                /*
                    در صورتی که فیلد آخرین کد روی فرم تنظیمات مقدار داشته باشد، این مقدار را می خوانید
                    و به آن یکی اضافه کرده و در فیلد کد روی فرم مورد نظر قرارداده و مقدار آخرین کد را نیز به روز نمائید
                 */
                if (settings.Attributes.Contains("new_lastcode"))
                {
                    //خواندن مقدار فیلد آخرین کد روی فرم تنظیمات
                    maxCode = settings.Attributes["new_lastcode"];

                    // واکشی اعداد از رشته
                    var numbers = string.Concat(maxCode.ToString().Where(char.IsNumber));

                    //ایجاد کد جدید
                    var newCounterValue1 = (Convert.ToInt32(numbers) + 1).ToString().PadLeft(Convert.ToInt32(lengthCode), '0');

                    // حذف مقدار قبلی فیلد کد در فرم تنظیمات
                    settings.Attributes.Remove("new_lastcode");
                    crmService.Update(settings);

                    // به روزرسانی فیلد آخرین کد روی فرم تنظیمات
                    settings.Attributes.Add("new_lastcode", preCode.ToString() + separator + newCounterValue1.ToString());
                    crmService.Update(settings);

                    // ایجاد کد جدید روی فرم مورد نظر
                    entity.Attributes.Add(AttributeName.Get(localWorkflowExecution.executeContext), settings.Attributes["new_lastcode"]);
                    crmService.Update(entity);
                }

                /*
                 *  در صورتی که فیلد آخرین کد روی فرم تنظیمات مقدار نداشته باشد، باید دو حالت زیر بررسی شود:
                 *  1- اگر هیچ کدی روی فرم مورد نظر ایجاد نشده بود، بایستی اولین کد متناسب با ورودی ها ایجاد شود
                 *  2- اگر از قبل یکسری کد ایجاد شده بود بایستی آخرین کد خوانده شده و یکی به آن اضافه شود
                 */
                else
                {
                    /// <summary>
                    /// کوئری مربوط به خواندن آخرین کد موجود روی فرم مورد نظر
                    /// </summary>
                    QueryExpression qe = new QueryExpression(entityName);
                    FilterExpression fe = new FilterExpression();
                    qe.ColumnSet = new ColumnSet(AttributeName.Get(localWorkflowExecution.executeContext));
                    qe.Orders.Add(new OrderExpression(AttributeName.Get(localWorkflowExecution.executeContext), OrderType.Descending));
                    var countContract = crmService.RetrieveMultiple(qe).Entities.First();

                    Entity AutoPost = crmService.Retrieve(entity.LogicalName, countContract.Id, new ColumnSet(AttributeName.Get(localWorkflowExecution.executeContext)));
                    var currentrecordcounternumber = AutoPost.GetAttributeValue<string>(AttributeName.Get(localWorkflowExecution.executeContext));

                    // اگر کد از قبل ایجاد شده بود
                    if (currentrecordcounternumber != null)
                    {
                        // واکشی اعداد از کد خوانده شده
                        var lencurrentrecordcounternumber = currentrecordcounternumber.Length - Convert.ToInt32(lengthCode);
                        var currentrecordcounternumbers = currentrecordcounternumber.Substring(lencurrentrecordcounternumber, Convert.ToInt32(lengthCode));

                        //محاسبه کد جدید
                        var numbers = string.Concat(currentrecordcounternumbers.Where(char.IsNumber));
                        var lenNum = numbers.Length;

                        var newCounterValue = (Convert.ToInt32(numbers) + 1).ToString().PadLeft(lenNum, '0');

                        // به روزرسانی فیلد کد روی فرم مورد نظر
                        entity.Attributes.Add(AttributeName.Get(localWorkflowExecution.executeContext), preCode.ToString() + separator.ToString() + newCounterValue.ToString());
                        crmService.Update(entity);

                        // به روزرسانی فیلد آخرین کد روی فرم تنظیمات
                        settings.Attributes.Add("new_lastcode", preCode.ToString() + separator.ToString() + newCounterValue.ToString());
                        crmService.Update(settings);
                    }
                    // اگر فرم تازه ایجاد شده باشد
                    else
                    {
                        // ایجاد کد
                        var newCounter = Convert.ToInt32(code).ToString().PadLeft(Convert.ToInt32(lengthCode), '0');

                        // به روزرسانی فیلد کد روی فرم مورد نظر
                        entity.Attributes.Add(AttributeName.Get(localWorkflowExecution.executeContext), preCode.ToString() + separator.ToString() + newCounter);
                        crmService.Update(entity);

                        // به روزرسانی فیلد آخرین کد روی فرم تنظیمات
                        settings.Attributes.Add("new_lastcode", preCode.ToString() + separator.ToString() + newCounter);
                        crmService.Update(settings);
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