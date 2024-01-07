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

<div dir = "ltr">

- [نیازمندی های پروژه](#نیازمندی-های-پروژه)
- [نحوه نصب پکیج ها در پروژه](#نحوه-نصب-پکیج-ها-در-پروژه)
- [Create a Visual Studio project for the plug-in](#create-a-visual-studio-project-for-the-plug-in)
- [Edit the class file to enable a plug-in](#edit-the-class-file-to-enable-a-plug-in)

</div>

---

<div dir="rtl" style="font-family:IranSans;">

<p style="direction:rtl; text-align:right;" dir="rtl">

# نیازمندی های پروژه
* 1-  Frame Work های زیر را به پروژه اضافه نمایید:

</p>

<div dir="ltr">

    * Microsoft.CrmSdk.CoreAssemblies
    * Microsoft.CrmSdk.CoreTools
    * Microsoft.CrmSdk.Workflow
    * Microsoft.CrmSdk.XrmTooling.PluginRegistrationTool

</div>

**تذکر:**
* Microsoft.CrmSdk.Workflow پکیج فقط در پلاگین های وابسته به گردش کار مورد استفاده قرار می گیرد.

* 2-  NameSpace های زیر را در File Class اضافه نمایید.
<br />

<div dir="ltr">

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

<div dir="rtl">

# نحوه نصب پکیج ها در پروژه
* جهت نصب پکیج ها در پروژه می توانید به روش های زیر عمل نمایید:

1- Tools > NuGet Package Manager > Package Manager Console

2- Command زیر را در PowerShell بنویسید:

<div style = "direction:ltr; text-align:left;" dir="ltr">

    * Install-Package Microsoft.CrmSdk.CoreAssemblies

</div>

3- همچنین می توانید روی پروژه کلیک راست کرده و گزینه Manage NuGet Packages را انتخاب نمایید.

</div>

---


# Create a Visual Studio project for the plug-in
<div dir="ltr" style="font-family:IranSans;">

1. Open Visual Studio and open a new Class Library (.NET Framework) project using .NET Framework 4.6.2

<div style="margin:auto; text-align:center;">

![Alt text](Images/image.png)

</div>

> The name used for the project will be the name of the assembly. This tutorial uses the name BasicPlugin.

2. In Solution Explorer, right-click the project and select Manage NuGet Packages… from the context menu.

<div style="margin:auto; text-align:center;">

![Alt text](Images/image-1.png)

</div>

3. Select Browse and search for Microsoft.CrmSdk.CoreAssemblies and install the latest version.

<div style="margin:auto; text-align:center;">

![Alt text](Images/image-2.png)

</div>

4. You must select I Accept in the License Acceptance dialog.

**Note**

> Adding the Microsoft.CrmSdk.CoreAssemblies NuGet package will include these assemblies in the build folder for your assembly, but you will not upload these assemblies with the assembly that includes your logic. These assemblies are already present in the sandbox runtime.

> Do not include any other NuGet packages or assemblies to the build folder of your project. You cannot include these assemblies when you register the assembly with your logic. You cannot assume that the assemblies other than those included in the Microsoft.CrmSdk.CoreAssemblies NuGet package will be present on the server and compatible with your code.

5. In Solution Explorer, right-click the Class1.cs file and choose Rename in the context menu.

<div style="margin:auto; text-align:center;">

![Alt text](Images/image-3.png)

</div>

6. Rename the Class1.cs file to FollowupPlugin.cs.

7. When prompted, allow Visual Studio to re-name the class to match the file name.

<div style="margin:auto; text-align:center;">

![Alt text](Images/image-4.png)

</div>


<div dir="ltr" style="font-family:IranSans;">

# Edit the class file to enable a plug-in

1. Add the following using statements to the top of the FollowupPlugin.cs file:

    * using System.ServiceModel;  
    * using Microsoft.Xrm.Sdk;

2. Implement the IPlugin interface by editing the class.

**Note**
> If you just type : IPlugin after the class name, Visual Studio will auto-suggest implementing a stub for the Execute Method.

<div dir="ltr" style="font-family:IranSans;">

        public class FollowupPlugin : IPlugin
        {
            public void Execute(IServiceProvider serviceProvider)
            {
                throw new NotImplementedException();
            }
        }   

</div>

3. Replace the contents of the Execute method with the following code:

<div dir="ltr" style="font-family:IranSans;">

    // Obtain the tracing service
    ITracingService tracingService =(ITracingService)serviceProvider.GetService(typeof(ITracingService));

    // Obtain the execution context from the service provider.  
    IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

    // The InputParameters collection contains all the data passed in the message request.  
    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
    {
        // Obtain the target entity from the input parameters.  
        Entity entity = (Entity)context.InputParameters["Target"];

        // Obtain the IOrganizationService instance which you will need for  
        // web service calls.  
        IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
        IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

        try
        {
            // Plug-in business logic goes here.  
        }
		
		catch (FaultException<OrganizationServiceFault> ex)
        {
            throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
        }

        catch (Exception ex)
        {
            tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
            throw;
        }
    }

</div>

**<div dir="rtl">نکته:</div>**
<div dir = "rtl" style="font-family:IranSans;">

> برای بهینه سازی کدها ساختار پروژه به گونه طراحی گردیده که در یک کلاس به نام WorkflowBase یا PluginBase اتصال به SDK مربوط Dynamics 365 انجام شده و در بقیه پروژه آن کلاس override می شود.

</div>

</div>


</div>

</div>