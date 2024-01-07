<div dir="rtl" style="font-family:IranSans;">

<div style = "font-size:20px; font-weight:bold; margin-bottom:10px; direction:ltr; text-align:right;" dir="rtl"> Publish </div>

<p style="direction:rtl; text-align:right;" dir="rtl">

> پروژه WF_XRM_CreateNumberAuto.cs برای ایجاد کد جدید روی فیلد در هنگام ایجاد یک رکورد ایجاد شده است. نیاز بود تا هنگامی یک رکورد جدید در یک فرم ایجاد می شود، یک فیلد به عنوان کد انحصاری آن رکورد مقداردهی شود. برای این منظور نیاز است تا مقدار آخرین رکورد دریافت شده و یکی به مقدار آن اضافه گردد و در فیلد مربوط به رکورد جدید درج گردد.

> بنابراین این پروژه به صورت پلاگین وابسته به گردش کار ایجاد گردید تا بتواند به صورت داینامیک روی هر فرمی ایجاد شود. برای استفاده از این پلاگین نیاز است تا روی هر سامانه یک فرم به نام تنظیمات (new_settings) ایجاد شده و فیلدهای زیر روی آن تعریف گردد:

**نام فیلدها:**
1. نام موجودیت new_name (String)
2. پیشوند new_prefixcode (String)
3. جداکننده کد new_separatorcode (String)
4. شمارنده new_countercode (Number)
5. طول پسوند کد new_lengthsuffixcode (Number)
6. آخرین کد new_lastcode (String)

> برای دریافت جزئیات بیشتر درخصوص جزئیات این پروژه به [Wiki](http://192.168.91.38:6600/IndexCollection/SoftDev/_wiki/wikis/SoftDev.wiki/2/WF_XRM_CreateNumberAuto) پروژه مراجعه نمائید.

</p>

</div>

---

<div dir="rtl" style="font-family:IranSans;">

<div style = "font-size:20px; font-weight:bold; margin-bottom:10px; direction:ltr; text-align:right;" dir="rtl"> Introduction </div>

<div style = "font-size:15px; font-weight:bold; margin-bottom:10px; direction:ltr; text-align:right;" dir="rtl"> WF_XRM_CreateNumberAuto </div>

<p style="direction:rtl; text-align:justify;" dir="rtl">

> هدف از انجام این پروژه ایجاد کد انحصاری به صورت اتوماتیک برای هر رکورد به صورت داینامیک می باشد تا هر رکورد از رکورد دیگر متمایز گردد، به این صورت که به آخرین کد ایجاد شده یک عدد اضافه شده و به رکورد جدید انتساب داده می شود.
</p>

</div>

---

<div style = "font-size:20px; font-weight:bold; margin-bottom:10px; direction:ltr; text-align:left; font-family:IranSans;" dir="rtl"> Technologies </div>

    * Class Library (.Net Framework 4.6.2)
    * CoreAssemblies
    * CoreTools
    * Workflow
    * PluginRegistrationTool
    * Microsoft Dynamics 365

</div>

---

<div style = "font-size:28px; font-weight:bold; font-family:IranSans;" dir = "rtl">  محتویات فایل ReadMe.md </div>

<div dir = "ltr" style="font-family:IranSans;">

- [نیازمندی های پروژه](#نیازمندی-های-پروژه)
- [نحوه نصب پکیج ها در پروژه](#نحوه-نصب-پکیج-ها-در-پروژه)
- [Code Snippet](#code-snippet)
- [Sources](#sources)

</div>

---

<div dir="rtl" style="font-family:IranSans;">

<p style="direction:rtl; text-align:right;" dir="rtl">

# نیازمندی های پروژه
* 1-  Frame Work های زیر را به پروژه اضافه نمایید:

</p>

<div dir="ltr" style="font-family:IranSans;">

    * Microsoft.CrmSdk.CoreAssemblies

    * Microsoft.CrmSdk.CoreTools

    * Microsoft.CrmSdk.Workflow

    * Microsoft.CrmSdk.XrmTooling.PluginRegistrationTool

</div>

* 2-  NameSpace های زیر را در File Class اضافه نمایید.

<div dir="ltr" style="font-family:IranSans;">

    //namespaces for d365 interaction
	
	* using Microsoft.Xrm.Sdk;
    * using Microsoft.Xrm.Sdk.Workflow;
    * using System.Activities;   
    * using Microsoft.Xrm.Sdk.Query;    
    * using System.Globalization;
    * using Customization_XRM.Base.WorkFlowBase;
    * using Customization_XRM.Base.Common_URL;
    * using System.Threading;

</div>

---

<div dir="rtl" style="font-family:IranSans;">

# نحوه نصب پکیج ها در پروژه
* جهت نصب پکیج ها در پروژه می توانید به روش های زیر عمل نمایید:

1- Tools > NuGet Package Manager > Package Manager Console

2- Command زیر را در PowerShell بنویسید:

<div style = "direction:ltr; text-align:left; font-family:IranSans;" dir="ltr">

    * Install-Package Microsoft.CrmSdk.CoreAssemblies

</div>

3- همچنین می توانید روی پروژه کلیک راست کرده و گزینه Manage NuGet Packages را انتخاب نمایید.

</div>

---

# Code Snippet
<div dir = "rtl" style="font-family:IranSans;">

> در ادامه کلیه کدهای پروژه را مرحله به مرحله شرح خواهیم داد. 

1. در مرحله اول بایستی از Interface مربوط به اتصال به SDK Dynamics 365 که با نام WorkflowBase ایجاد نمودیم، ارث بری نمائید:

<div dir = "ltr" style="font-family:IranSans;">

        public WF_XRM_CreateNumberAuto() : base(typeof(WF_XRM_CreateNumberAuto))
        {

        }    
</div>

**نکته:**
> به علت ارث بری از Interface WorkFlowBase لازم است هم Constructor پروژه را ایجاد نمائید و هم Interface را Override نمائید:

<div dir = "ltr" style="font-family:IranSans;">

        public class WF_XRM_CreateNumberAuto : WorkflowBase
        {
        
        }

        protected override void ExecuteWorkFlowLogic(LocalWorkflowExecution localWorkflowExecution)
        {

        }
</div>

2.  از آن جایی که پروژه از نوع پلاگین وابسته به گردش کار می باشد، بنابراین نیاز است تا یکسری ورودی از طریق گردش کار کار به داخل پلاگین پاس داده شود. ورودی ها را برای تمیزی کد قبل از متد ExecuteWorkFlowLogic تعریف نمائید:

<div dir = "ltr" style="font-family:IranSans;">

        // تعریف ورودی های گردش کار
        //که به طور پیش فرض روی همه فرم ها ایجاد می شود { Record URL(Dynamic)(...)} مقدار فیلد 
        [RequiredArgument]
        [Input("Record URL")]
        [ReferenceTarget("")]
        public InArgument<String> RecordURL { get; set; }

        //ورودی برای گرفتن آیدی فرم تنظیمات
        [Input("Setting ID")]
        [ReferenceTarget("new_settings")]
        public InArgument<EntityReference> SettingID { get; set; }

        //ورودی برای گرفتن نام فیلدی که قرار است کد روی آن ایجاد شود
        [Input("Attribute Name")]
        public InArgument<string> AttributeName { get; set; }
</div>

3. حال بدنه اصلی کد به شرح زیر می باشد:

<div dir = "ltr" style="font-family:IranSans;">

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
                Entity settings = crmService.Retrieve("new_Settings", SettingID.Get(localWorkflowExecution.executeContext).Id,
                                                              new ColumnSet("new_pre_code", "new_length_code", "new_code", "new_separator"));

                //Get Value From Settings
                var preCode = settings.Attributes["new_pre_code"];
                var lengthCode = settings.Attributes["new_length_code"];
                var code = settings.Attributes["new_code"];
                var separator = settings.Attributes["new_separator"];

                Object maxCode;

                /*
                    در صورتی که فیلد آخرین کد روی فرم تنظیمات مقدار داشته باشد، این مقدار را می خوانید
                    و به آن یکی اضافه کرده و در فیلد کد روی فرم مورد نظر قرارداده و مقدار آخرین کد را نیز به روز نمائید
                 */
                if (settings.Attributes.Contains("new_max_code"))
                {
                    //خواندن مقدار فیلد آخرین کد روی فرم تنظیمات
                    maxCode = settings.Attributes["new_max_code"];

                    // واکشی اعداد از رشته
                    var numbers = string.Concat(maxCode.ToString().Where(char.IsNumber));

                    //ایجاد کد جدید
                    var newCounterValue1 = (Convert.ToInt32(numbers) + 1).ToString().PadLeft(Convert.ToInt32(lengthCode), '0');

                    // حذف مقدار قبلی فیلد کد در فرم تنظیمات
                    settings.Attributes.Remove("new_max_code");
                    crmService.Update(settings);

                    // به روزرسانی فیلد آخرین کد روی فرم تنظیمات
                    settings.Attributes.Add("new_max_code", preCode.ToString() + separator + newCounterValue1.ToString());
                    crmService.Update(settings);

                    // ایجاد کد جدید روی فرم مورد نظر
                    entity.Attributes.Add(AttributeName.Get(localWorkflowExecution.executeContext), settings.Attributes["new_max_code"]);
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
                        settings.Attributes.Add("new_max_code", preCode.ToString() + separator.ToString() + newCounterValue.ToString());
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
                        settings.Attributes.Add("new_max_code", preCode.ToString() + separator.ToString() + newCounter);
                        crmService.Update(settings);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

</div>

</div>

---

# Sources

<div style="text-align:left; direction:ltr; font-family:IranSans;" dir="ltr">
<p>

* https://lunalearn.ir/product/%d9%be%d9%84%d8%a7%da%af%db%8c%d9%86-%d9%86%d9%88%db%8c%d8%b3%db%8c-%d9%85%d8%a7%db%8c%da%a9%d8%b1%d9%88%d8%b3%d8%a7%d9%81%d8%aa-crm/

* https://blog.magnetismsolutions.com/blog/paulnieuwelaar/2016/04/12/queryexpression-filter-lookup-by-name-instead-of-id-crm-2016

* https://learn.microsoft.com/en-us/power-apps/developer/data-platform/tutorial-write-plug-in

* https://learn.microsoft.com/en-us/power-apps/developer/data-platform/org-service/use-filterexpression-class

* https://stackoverflow.com/questions/68541167/how-to-put-and-and-or-filter-type-when-retrievemultiple-query-is-fetch-expressio

* https://www.dideo.tv/v/yt/cabRANPQBvU/%D8%AA%D9%85%D8%A7%D8%B4%D8%A7%DB%8C-%D9%88%DB%8C%D8%AF%D8%A6%D9%88-%D8%A7%D8%B2-%D8%AF%DB%8C%D8%AF%D8%A6%D9%88-watch-video-from-dideo

* https://carldesouza.com/dynamics-365-pre-operation-plugin/

* https://learn.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.query.queryexpression?view=dataverse-sdk-latest&viewFallbackFrom=dynamics-general-ce-9

</p>

</div>