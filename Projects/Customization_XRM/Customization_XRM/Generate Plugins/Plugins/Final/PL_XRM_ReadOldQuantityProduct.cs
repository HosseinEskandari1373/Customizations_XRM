using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// namespace for d365 
using CRM;
using Customization_XRM.Base.PluginBase;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Customization_XRM.Generate_Plugins.Plugins.Final
{
    public class PL_XRM_ReadOldQuantityProduct : PluginBase
    {
        public PL_XRM_ReadOldQuantityProduct() : base(typeof(PL_XRM_ReadOldQuantityProduct))
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