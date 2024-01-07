<div dir="rtl" style="font-family:IranSans;">

<div style = "font-size:20px; font-weight:bold; margin-bottom:10px; direction:ltr; text-align:right;" dir="rtl"> Publish </div>

<p style="direction:rtl; text-align:right;" dir="rtl">

> پروژه WF_XRM_Confirmation_TTL_Time.cs به منظور محاسبه مجموع زمان هر تأیید ایجاد شده است. بدین منظور جهت داینامیک بودن این پلاگین به نحوی که روی هر سامانه قابل استفاده باشد، این پلاگین از نوع پلاگین وابسته به گردش کار ایجاد شود تا بتوان در گردش کار از آن استفاده نمود. نحوه نوشتن گردش کار در [Wiki](https://azure.index-holding.com/IndexCollection/SoftDev/_wiki/wikis/SoftDev_wiki/35/WF_XRM_Confirmation_TTL_Time) شرح داده شده است.

> بدین منظور نیاز است تا ورودی های زیر برای پلاگین دریافت گردد: 
> 1- **Record URL:**   
> - مقدار فیلد { Record URL(Dynamic)(...)} که به طور پیش فرض روی همه فرم ها ایجاد می شود، را در این ورودی وارد نمائید.
>
> 2- **Confirmed Field Name:**
> - نام فیلد تأیید روی سامانه اطلاعاتی XRM را برای این ورودی وارد کنید.
> 
> 3- **Sum Date Confirm Field:**
> - نام فیلد مجموع زمان تأییدات روی سامانه های اطلاعاتی XRM را در این ورودی ورود نمائید.
> 
> 4- **Confirm Date Field Name:**
> - نام فیلد آخرین تاریخ تأیید هر فیلد که از طریق گردش کار مقداردهی می شود، را در این ورودی وارد نمائید.

**نکته:**
- از آن جایی که جهت محاسبه مجموع زمان تأییدات نیاز است تا تاریخ ثبت تأیید، از آخرین تاریخی که تأیید ثبت شده است، کم شده و با مقدار قبل فیلد مجموع زمان جمع گردد، بنابراین باید ابتدا بررسی شود تا درصورتی که فیلد آخرین تاریخ تأییدات شامل مقدار نبود، تاریخ ثبت تأیید از زمان ایجاد رکورد کم شده و مقدار آن برحسب دقیقه با مقدار قبل فیلد مجموع زمان جمع گردد و در غیر این صورت تاریخ ثبت رکورد باید از آخرین تاریخ تأییدات کم شود.
<br />
- حال با توجه به توضیحات بالا نیاز است تا در گردش کار پس از هر بار اجرای پلاگین، فیلد آخرین تاریخ تأییدات از طریق گردش کار مقداردهی شود.

> برای دریافت جزئیات بیشتر درخصوص جزئیات این پروژه به [Wiki](https://azure.index-holding.com/IndexCollection/SoftDev/_wiki/wikis/SoftDev_wiki/35/WF_XRM_Confirmation_TTL_Time) پروژه مراجعه نمائید.

</p>

</div>

---

<div dir="rtl" style="font-family:IranSans;">

<div style = "font-size:20px; font-weight:bold; margin-bottom:10px; direction:ltr; text-align:right;" dir="rtl"> Introduction </div>

<div style = "font-size:15px; font-weight:bold; margin-bottom:10px; direction:ltr; text-align:right;" dir="rtl"> WF_XRM_Confirmation_TTL_Time </div>

<p style="direction:rtl; text-align:justify;" dir="rtl">

> هدف از ایجاد این پلاگین، محاسبه مجموع زمان تأییدات برای هر فیلد تأیید می باشد تا بتوان بررسی نمود که هر شخص چه میزان زمانی را صرف بررسی تأیید نموده است.
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
</div>

3. حال بدنه اصلی کد به شرح زیر می باشد:

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
            public class WF_XRM_Confirmation_TTL_Time : WorkflowBase
            {
                public WF_XRM_Confirmation_TTL_Time() : base(typeof(WF_XRM_Confirmation_TTL_Time))
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