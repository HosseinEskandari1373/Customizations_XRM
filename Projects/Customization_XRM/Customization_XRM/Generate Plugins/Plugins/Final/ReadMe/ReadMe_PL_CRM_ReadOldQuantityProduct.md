<div dir="rtl" style="font-family:IranSans;">

<div style = "font-size:20px; font-weight:bold; margin-bottom:10px; direction:ltr; text-align:right;" dir="rtl"> Publish </div>

<p style="direction:rtl; text-align:right;" dir="rtl">

> پروژه PL_CRM_ReadOldQuantityProduct.cs به منظور خواندن آخرین مقدار قبلی ثبت شده برای محصول فرصت مرتبط با محصول فرصت ایجاد شده می باشد.

> بدین منظور امکان ایجاد این پروژه از نوع پلاگین وابسته به گردش کار نبوده و این پلاگین بایستی هنگام ایجاد محصول فرصت اجرا گردد. پس از اجرای پلاگین نیاز است تا شخص یا شرکت مرتبط با محصول فرصت ایجاد شده را خوانده و سپس لیست فرصت های آن را جهت رسیدن به آخرین مقدار فیلد مقدار قبلی از روی محصول فرصت های آن ها به دست آورید.

> برای دریافت جزئیات بیشتر درخصوص جزئیات این پروژه به [Wiki](https://azure.index-holding.com/IndexCollection/SoftDev/_wiki/wikis/SoftDev_wiki/64/PL_CRM_ReadOldQuantityProduct) پروژه مراجعه نمائید.

</p>

</div>

---

<div dir="rtl" style="font-family:IranSans;">

<div style = "font-size:20px; font-weight:bold; margin-bottom:10px; direction:ltr; text-align:right;" dir="rtl"> Introduction </div>

<div style = "font-size:15px; font-weight:bold; margin-bottom:10px; direction:ltr; text-align:right;" dir="rtl"> PL_CRM_ReadOldQuantityProduct </div>

<p style="direction:rtl; text-align:justify;" dir="rtl">

> هدف از ایجاد این پلاگین قرار دادن آخرین مقدار ثبت شده برای فیلد مقدار قبلی روی محصول فرصت ایجاد شده می باشد.
</p>

</div>

---

<div style = "font-size:20px; font-weight:bold; margin-bottom:10px; direction:ltr; text-align:left; font-family:IranSans;" dir="rtl"> Technologies </div>

    * Class Library (.Net Framework 4.6.2)
    * CoreAssemblies
    * CoreTools
    * PluginRegistrationTool
    * Microsoft Dynamics 365
    * MyCRM
    * Linq

</div>

---

<div style = "font-size:28px; font-weight:bold; font-family:IranSans;" dir = "rtl">  محتویات فایل ReadMe.md </div>

<div dir = "ltr" style="font-family:IranSans;">

- [نیازمندی های پروژه](#نیازمندی-های-پروژه)
- [نحوه نصب پکیج ها در پروژه](#نحوه-نصب-پکیج-ها-در-پروژه)
- [نحوه گرفتن خروجی کلاس از سامانه های اطلاعاتی](#نحوه-گرفتن-خروجی-کلاس-از-سامانه-های-اطلاعاتی)
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

    * Microsoft.CrmSdk.XrmTooling.PluginRegistrationTool

</div>

* 2-  NameSpace های زیر را در File Class اضافه نمایید.

<div dir="ltr" style="font-family:IranSans;">

    //namespaces for d365 interaction
    * using System;
    * using System.Collections.Generic;
    * using System.Linq;

    //namespaces for d365 interaction
    * using Microsoft.Xrm.Sdk;
    * using Microsoft.Xrm.Sdk.Query;
    * using Customization_XRM.Base.PluginBase;
    * using MyCRM;

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

<div dir="rtl" style="font-family:IranSans;">

# نحوه گرفتن خروجی کلاس از سامانه های اطلاعاتی

1. ابتدا وارد پوشه coretools شوید و کلمه CMD را در نوارد بالا وارد کنید تا CMD در آن آدرس باز شود.

<div style="text-align:center; margin-top: 20px;">

![Alt text](image.png)

</div>

2. سپس کد زیر را در CMD وارد کنید و سپس تنظیمات لازم جهت متصل شدن به سامانه را متناسب با آموزش های ضبط شده وارد کنید تا خروجی کلاس برای شما ایجاد شود:

<div dir = "ltr" style="font-family:IranSans;">

        CrmSvcUtil.exe /interactivelogin /out:CRM.cs /namespace:MyCRM /serviceContextName:XrmServiceContext /generateActions

</div>

</div>

---


# Code Snippet
<div dir = "rtl" style="font-family:IranSans;">

> در ادامه کلیه کدهای پروژه را مرحله به مرحله شرح خواهیم داد. 

1. در مرحله اول بایستی از Interface مربوط به اتصال به SDK Dynamics 365 که با نام WorkflowBase ایجاد نمودیم، ارث بری نمائید:

<div dir = "ltr" style="font-family:IranSans;">

        protected override void ExecutePluginLogic(LocalPluginExecution localPluginExecution)
        {

        }    
</div>

**نکته:**
> به علت ارث بری از Interface PluginBase لازم است هم Constructor پروژه را ایجاد نمائید و هم Interface را Override نمائید:

<div dir = "ltr" style="font-family:IranSans;">

        public class PL_CRM_ReadOldQuantityProduct : PluginBase
        {
        
        }

        protected override void ExecutePluginLogic(LocalPluginExecution localPluginExecution)
        {

        }
</div>

2. حال بدنه اصلی کد به شرح زیر می باشد:

<div dir = "ltr" style="font-family:IranSans;">

        using System;
        using System.Collections.Generic;
        using System.Linq
        //namespaces for d365 interaction
        using Microsoft.Xrm.Sdk;
        using Microsoft.Xrm.Sdk.Query;
        using Customization_XRM.Base.PluginBase;
        using MyCRM;

        namespace Customization_XRM.Plugins.Read_Old_Quantitty_Product
        {
            public class PL_CRM_ReadOldQuantityProduct : PluginBase
            {
                public PL_CRM_ReadOldQuantityProduct() : base(typeof(PL_CRM_ReadOldQuantityProduct))
                {

                }

                // متغیر برای فراخوانی آیدی محصول فرصت ایجاد شده
                EntityReference FLookUpContactTarget;
				
				// متغیر برای فراخوانی شخص مرتبط با محصول فرصت ایجاد شده
                EntityReference FLookUpContact;
                Contact contact;

                // متغیر برای فراخوانی شرکت مرتبط با محصول فرصت ایجاد شده
                EntityReference FLookUpAccount;
                Account account;


                protected override void ExecutePluginLogic(LocalPluginExecution localPluginExecution)
                {
                    try
                    {
                        //مقدار نداشته باشد، یعنی اتصال صورت نگرفته و خطا رخ داده است localWorkflowExecution در صورتی که 
                        if (localPluginExecution == null)
                        {
                            throw new InvalidPluginExecutionException("Local Plugin Execution is not initialized correctly.");
                        }

                        /// <summary>
                        ///  initialize plugin basic components
                        /// </summary>  

                        // Obtain the tracing service
                        ITracingService tracingService = localPluginExecution.tracingService;

                        // Obtain the execution context from the service provider.  
                        IPluginExecutionContext context = localPluginExecution.pluginContext;

                        // Obtain the IOrganizationService instance which you will need for web service calls.  
                        IOrganizationService crmService = localPluginExecution.orgService;

                        // بررسی ایجاد یا به روزرسانی محصول فرصت
                        if (context.InputParameters.Contains("Target") &&
                                context.InputParameters["Target"] is Entity)
                        {
                            // فراخوانی رکورد محصول فرصت ایجاد شده
                            Entity entity = (Entity)context.InputParameters["Target"];

                            // تبدیل رکورد محصول فرصت خوانده شده به نوع محصول فرصت
                            OpportunityProduct opportunityProduct = crmService.Retrieve(OpportunityProduct.EntityLogicalName, entity.Id, new ColumnSet(true)) as OpportunityProduct;
                            // فرخوانی آیدی محصول فرصت ایجاد شده
                            FLookUpContactTarget = opportunityProduct.ProductId;

                            //خواندن فرصت مربوط به محصول مورد نیاز 
                            var lookUpOpurMain = opportunityProduct.OpportunityId;
                            Opportunity opportunity = crmService.Retrieve(lookUpOpurMain.LogicalName, lookUpOpurMain.Id, new ColumnSet(true)) as Opportunity;

                            // مربوط به شخص مرتبط محصول فرصت ایجاد شده Guid متغیر برای فراخوانی 
                            Guid contactID;
                            // متغیر برای فراخوانی نام شخص مرتبط با محصول فرصت ایجاد شده
                            string contactName = "";

                            // مربوط به شرکت مرتبط محصول فرصت ایجاد شده Guid متغیر برای فراخوانی 
                            Guid accountID;
                            // متغیر برای فراخوانی نام شرکت مرتبط با محصول فرصت ایجاد شده
                            string accountName = "";

                            /// 
                            /// خواندن آیدی شخص مربوط به فرصت
                            ///
                            // اگر شخص مرتبط با محصول فرصت ایجاد شده مقدار داشت
                            if (opportunity.ParentContactId != null)
                            {
                                // فراخوانی شخص مرتبط با محصول فرصت ایجاد شده
                                FLookUpContact = opportunity.ParentContactId;
                                contact = crmService.Retrieve(FLookUpContact.LogicalName, FLookUpContact.Id, new ColumnSet(true)) as Contact;

                                contactID = contact.Id;
                                contactName = contact.FullName;
                            }
                            else
                            {
                                // فراخوانی شرکت مرتبط با محصول فرصت ایجاد شده
                                FLookUpAccount = opportunity.ParentAccountId;
                                account = crmService.Retrieve(FLookUpAccount.LogicalName, FLookUpAccount.Id, new ColumnSet(true)) as Account;

                                accountID = account.Id;
                                accountName = account.Name;
                            }

                            // اگر شخص بود
                            if (contact != null)
                            {
                                // فراخوانی شخص مرتبط با محصول فرصت ایجاد شده و تبدیل آن به نوع شخص
                                Contact readContact = crmService.Retrieve(contact.LogicalName, contact.Id, new ColumnSet(true)) as Contact;
								
								// به دست آورن لیست فرصت های مرتبط با شخص
                                QueryExpression contactOpurQuery = new QueryExpression
                                {
                                    EntityName = Opportunity.EntityLogicalName,
                                    ColumnSet = new ColumnSet(true),
                                    Criteria = new FilterExpression
                                    {
                                        Conditions =
                                        {
                                            new ConditionExpression
                                            {
                                                AttributeName = "parentcontactid",
                                                Operator = ConditionOperator.Equal,
                                                Values = { readContact.Id}
                                            }
                                        }
                                    }
                                };

                                // خواندن لیست فرصت های مرتبط با شخص و تبدیل آن به نوع فرصت
                                IEnumerable<Opportunity> contactOpurs = crmService.RetrieveMultiple(contactOpurQuery).Entities.Cast<Opportunity>().ToList();

                                // لیست برای به دست آوردن لیست محصول فرصت های هر فرصت مرتبط با شخص
                                List<OpportunityProduct> listProductContact = new List<OpportunityProduct>();

                                // حلقه برای خواندن لیست محصول فرصت ها از لیست فرصت های مرتبط با شخص
                                foreach (var item in contactOpurs)
                                {
                                    // به دست آوردن محصول فرصت های مرتبط با هر فرصت
                                    QueryExpression contactProductQuery = new QueryExpression
                                    {
                                        EntityName = OpportunityProduct.EntityLogicalName,
                                        ColumnSet = new ColumnSet(true),
                                        Criteria = new FilterExpression
                                        {
                                            Conditions =
                                            {
                                                new ConditionExpression
                                                {
                                                    AttributeName = "opportunityid",
                                                    Operator = ConditionOperator.Equal,
                                                    Values = {item.Id}
                                                }
                                            }
                                        }
                                    };

                                    // خواندن لیست محصول فرصت های هر فرصت و تبدیل آن به نوع فرصت
                                    List<OpportunityProduct> contactProducts = crmService.RetrieveMultiple(contactProductQuery).Entities.Cast<OpportunityProduct>().ToList();

                                    // خواندن محصول فرصت های مرتبط با محصول فرصت ایجاد شده به شرطی که فیلد مقدار قبلی شامل مقدار باشد 
                                    IEnumerable<OpportunityProduct> selectNewOldQuantity = contactProducts.Where(p => p.new_old_quantity != null &&
                                                                                                                    p.ProductId.Id == FLookUpContactTarget.Id);

                                    // اگر محصول فرصت مرتبط با محصول فرصت ایجاد شده که فیلد مقدار قبلی آن شامل مقدار باشد، وجود داشت
                                    if (selectNewOldQuantity.Any())
                                    {
                                        // ذخیره سازی محصول فرصت مرتبط با محصول فرصت ایجاد شده که فیلد مقدار قبلی آن شامل مقدار باشد در یک لیست 
                                        listProductContact.AddRange(selectNewOldQuantity);
                                    }
                                }
								
								// خواندن مقدار قبلی های مربوطه از لیست
                                var checkValue = listProductContact.Where(p => p.ProductId.Id == FLookUpContactTarget.Id).Select(p => p.new_old_quantity);
                                // متغیر برای ذخیره سازی آخرین مقدار قبلی از لیست
                                int newOldQuantity;

                                // اگر در لیست حداقل یک مقدار قبلی ذخیره شده بود
                                if (checkValue.Any())
                                {
                                    // فراخوانی محصول فرصت های مرتبط با محصول فرصت ایجاد شده
                                    IEnumerable<OpportunityProduct> selectedTargetProductFromList = listProductContact.Where(p => p.ProductId.Id == FLookUpContactTarget.Id);
                                    // به دست آوردن آخرین تاریخ ایجاد محصول فرصت های ذخیره شده در لیست
                                    DateTime? maxCreatedOn = selectedTargetProductFromList.Max(p => p.CreatedOn);

                                    // فراخوانی آخرین محصول فرصت مرتبط با محصول فرصت ایجاد شده از لیست
                                    IEnumerable<OpportunityProduct> selectedResulrProduct = selectedTargetProductFromList.Where(p => p.CreatedOn == maxCreatedOn);
                                    // تبدیل لیست به یک رکورد
                                    OpportunityProduct selectOneRecord = selectedResulrProduct.FirstOrDefault();
                                    // فراخوانی مقدار فیلد مقدار قبلی از آخرین محصول فرصت مرتبط با محصول فرصت ایجاد شده و تبدیل آن به عدد
                                    newOldQuantity = Convert.ToInt32(selectOneRecord.new_old_quantity);
                                }
                                else
                                {
                                    // قرار دادن مقدار 0 برای محصول فرصتی که هیچ مقداری برای فیلد مقدار قبلی آن یافت نشد
                                    newOldQuantity = 0;
                                }

                                // به روزرسانی مقدار فیلد مقدار فیلد قبلی روی محصول فرصت ایجاد شده
                                crmService.Update(new OpportunityProduct { Id = entity.Id, new_old_quantity = newOldQuantity });
                            }
                            // اگر شرکت بود
                            else
                            {
                                // فراخوانی شرکت مرتبط با محصول فرصت ایجاد شده و تبدیل آن به نوع شخص
                                Account readAccount = crmService.Retrieve(account.LogicalName, account.Id, new ColumnSet(true)) as Account;

                                // به دست آورن لیست فرصت های مرتبط با شرکت
                                QueryExpression accountOpurQuery = new QueryExpression
                                {
                                    EntityName = Opportunity.EntityLogicalName,
                                    ColumnSet = new ColumnSet(true),
                                    Criteria = new FilterExpression
                                    {
                                        Conditions =
                                        {
                                            new ConditionExpression
                                            {
                                                AttributeName = "parentcontactid",
                                                Operator = ConditionOperator.Equal,
                                                Values = { readAccount.Id}
                                            }
                                        }
                                    }
                                };
								
								// خواندن لیست فرصت های مرتبط با شرکت و تبدیل آن به نوع فرصت
                                IEnumerable<Opportunity> accountOpurs = crmService.RetrieveMultiple(accountOpurQuery).Entities.Cast<Opportunity>().ToList();

                                // لیست برای به دست آوردن لیست محصول فرصت های هر فرصت مرتبط با شرکت
                                List<OpportunityProduct> listProductAccount = new List<OpportunityProduct>();

                                // حلقه برای خواندن لیست محصول فرصت ها از لیست فرصت های مرتبط با شرکت
                                foreach (var item in accountOpurs)
                                {
                                    // به دست آوردن محصول فرصت های مرتبط با هر فرصت
                                    QueryExpression accountProductQuery = new QueryExpression
                                    {
                                        EntityName = OpportunityProduct.EntityLogicalName,
                                        ColumnSet = new ColumnSet(true),
                                        Criteria = new FilterExpression
                                        {
                                            Conditions =
                                            {
                                                new ConditionExpression
                                                {
                                                    AttributeName = "opportunityid",
                                                    Operator = ConditionOperator.Equal,
                                                    Values = {item.Id}
                                                }
                                            }
                                        }
                                    };

                                    // خواندن لیست محصول فرصت های هر فرصت و تبدیل آن به نوع فرصت
                                    List<OpportunityProduct> accountProducts = crmService.RetrieveMultiple(accountProductQuery).Entities.Cast<OpportunityProduct>().ToList();

                                    // خواندن محصول فرصت های مرتبط با محصول فرصت ایجاد شده به شرطی که فیلد مقدار قبلی شامل مقدار باشد 
                                    IEnumerable<OpportunityProduct> selectNewOldQuantity = accountProducts.Where(p => p.new_old_quantity != null &&
                                                                                                                    p.ProductId.Id == FLookUpContactTarget.Id);

                                    // اگر محصول فرصت مرتبط با محصول فرصت ایجاد شده که فیلد مقدار قبلی آن شامل مقدار باشد، وجود داشت
                                    if (selectNewOldQuantity.Any())
                                    {
                                        // ذخیره سازی محصول فرصت مرتبط با محصول فرصت ایجاد شده که فیلد مقدار قبلی آن شامل مقدار باشد در یک لیست 
                                        listProductAccount.AddRange(selectNewOldQuantity);
                                    }
                                }

                                // خواندن مقدار قبلی های مربوطه از لیست
                                var checkValue = listProductAccount.Where(p => p.ProductId.Id == FLookUpContactTarget.Id).Select(p => p.new_old_quantity);
                                // متغیر برای ذخیره سازی آخرین مقدار قبلی از لیست
                                int newOldQuantity;

                                // اگر در لیست حداقل یک مقدار قبلی ذخیره شده بود
                                if (checkValue.Any())
                                {
                                    // فراخوانی محصول فرصت های مرتبط با محصول فرصت ایجاد شده
                                    IEnumerable<OpportunityProduct> selectedTargetProductFromList = listProductAccount.Where(p => p.ProductId.Id == FLookUpContactTarget.Id);
                                    // به دست آوردن آخرین تاریخ ایجاد محصول فرصت های ذخیره شده در لیست
                                    DateTime? maxCreatedOn = selectedTargetProductFromList.Max(p => p.CreatedOn);

                                    // فراخوانی آخرین محصول فرصت مرتبط با محصول فرصت ایجاد شده از لیست
                                    IEnumerable<OpportunityProduct> selectedResulrProduct = selectedTargetProductFromList.Where(p => p.CreatedOn == maxCreatedOn);
                                    // تبدیل لیست به یک رکورد
                                    OpportunityProduct selectOneRecord = selectedResulrProduct.FirstOrDefault();
                                    // فراخوانی مقدار فیلد مقدار قبلی از آخرین محصول فرصت مرتبط با محصول فرصت ایجاد شده و تبدیل آن به عدد
                                    newOldQuantity = Convert.ToInt32(selectOneRecord.new_old_quantity);
                                }
								else
                                {
                                    // قرار دادن مقدار 0 برای محصول فرصتی که هیچ مقداری برای فیلد مقدار قبلی آن یافت نشد
                                    newOldQuantity = 0;
                                }

                                // به روزرسانی مقدار فیلد مقدار فیلد قبلی روی محصول فرصت ایجاد شده
                                crmService.Update(new OpportunityProduct { Id = entity.Id, new_old_quantity = newOldQuantity });
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