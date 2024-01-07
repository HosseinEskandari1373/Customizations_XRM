<div dir="rtl" style="font-family:IranSans;">

<div style = "font-size:20px; font-weight:bold; margin-bottom:10px; direction:ltr; text-align:right;" dir="rtl"> Publish </div>

<p style="direction:rtl; text-align:right;" dir="rtl">

> پروژه WF_XRM_ReturnLookUpField به منظور رفع محدودیت عدم توانایی گردش کار در خواندن مقدار یک فیلد تا دو سطح پایین تر یک موجودیت ایجاد شده است. بدین منظور جهت داینامیک بودن این پلاگین به نحوی که روی هر سامانه قابل استفاده باشد، این پلاگین از نوع پلاگین وابسته به گردش کار ایجاد شده است تا بتوان در گردش کار از آن استفاده نمود. توضیحات بیشتر در [Wiki]( https://azure.index-holding.com/IndexCollection/SoftDev/_wiki/wikis/SoftDev_wiki/63/WF_XRM_ReturnLookUpField) شرح داده شده است.

> بدین منظور نیاز است تا ورودی های زیر برای پلاگین دریافت گردد: 
> 1- **Record URL:**   
> - مقدار فیلد { Record URL(Dynamic)(...)} که به طور پیش فرض روی همه فرم ها ایجاد می شود، را در این ورودی وارد نمائید.
>
> 2- **First LookUp Field Name:**
> - نام اولین فیلد LookUp.
> 
> 3- **Secound LookUp Field Name:**
> - نام دومین فیلد LookUp.
> 
> 4- **Target Field Name:**
> - نام فیلدی که از موجودیت در دو سطح پایین تر مورد نیاز می باشد.

برای دریافت جزئیات بیشتر درخصوص جزئیات این پروژه به [Wiki](https://azure.index-holding.com/IndexCollection/SoftDev/_wiki/wikis/SoftDev_wiki/63/WF_XRM_ReturnLookUpField) پروژه مراجعه نمائید.

</p>

</div>

---

<div dir="rtl" style="font-family:IranSans;">

<div style = "font-size:20px; font-weight:bold; margin-bottom:10px; direction:ltr; text-align:right;" dir="rtl"> Introduction </div>

<div style = "font-size:15px; font-weight:bold; margin-bottom:10px; direction:ltr; text-align:right;" dir="rtl"> WF_XRM_Confirmation_TTL_Time </div>

<p style="direction:rtl; text-align:justify;" dir="rtl">

> هدف از ایجاد این پلاگین خواندن مقدار فیلد از طریق فیلد LookUp تا دو سطح پایین تر و برگرداندن مقدار فیلد مورد نظر در خروجی پلاگین به افراد پشتیبان می باشد.
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
    * using Customization_XRM.Base.WorkFlowBase;
    * using Customization_XRM.Base.Common_URL;

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

        public WF_XRM_Confirmation_TTL_Time() : base(typeof(WF_XRM_Confirmation_TTL_Time))
        {

        }
</div>

**نکته:**
> به علت ارث بری از Interface WorkFlowBase لازم است هم Constructor پروژه را ایجاد نمائید و هم Interface را Override نمائید:

<div dir = "ltr" style="font-family:IranSans;">

        public class WF_XRM_Confirmation_TTL_Time : WorkflowBase
        {
        
        }

        protected override void ExecuteWorkFlowLogic(LocalWorkflowExecution localWorkflowExecution)
        {

        }
</div>

2.  از آن جایی که پروژه از نوع پلاگین وابسته به گردش کار می باشد، بنابراین نیاز است تا یکسری ورودی از طریق گردش کار کار به داخل پلاگین پاس داده شود. ورودی ها را برای تمیزی کد قبل از متد ExecuteWorkFlowLogic تعریف نمائید:

<div dir = "ltr" style="font-family:IranSans;">

        /********************************************************/
        /// <summary>
        /// تعریف ورودی های گردش کار
        /// </summary>

        //1) که به طور پیش فرض روی همه فرم ها ایجاد می شود { Record URL(Dynamic)(...)} مقدار فیلد 
        [RequiredArgument]
        [Input("Record URL")]
        [ReferenceTarget("")]
        public InArgument<String> RecordURL { get; set; }

        //2)  LookUp نام اولین فیلد
        [Input("First LookUp Field Name")]
        public InArgument<string> FirstLookupFiled { get; set; }

        //3)  LookUp نام دومین فیلد
        [Input("Secound LookUp Field Name")]
        public InArgument<string> SecoundLookupField { get; set; }

        //4)  نام فیلدی که از موجودیت در دو سطح پایین تر مورد نیاز می باشد.
        [Input("Target Field Name")]
        public InArgument<string> TargetField { get; set; }
</div>

خروجی ها را برای تمیزی کد بعد از متد ExecuteWorkFlowLogic تعریف نمائید:
<div dir = "ltr" style="font-family:IranSans;">

        /********************************************************/
        // OptionSet برای فیلد از نوع Int پارامتر خروجی از نوع
        [Output("OutputFieldOptionSetValue")]
        public OutArgument<int> OutputTargetFieldOptionSet { get; set; }

        // نمی باشند OptionSet پارامتر خروجی برای فیلدهایی که از نوع
        [Output("OutputFieldLookUpName")]
        public OutArgument<string> OutputTargetFieldLookUp { get; set; }
</div>

1. حال بدنه اصلی کد به شرح زیر می باشد:

<div dir = "ltr" style="font-family:IranSans;">

            using System;
            using System.Linq;

            //namespaces for d365 interaction
            using Microsoft.Xrm.Sdk;
            using Microsoft.Xrm.Sdk.Workflow;
            using System.Activities;
            using Microsoft.Xrm.Sdk.Query;
            using Customization_XRM.Base.WorkFlowBase;
            using Customization_XRM.Base.Common_URL;

            namespace Customization_XRM.Generate_Plugins
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