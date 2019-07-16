You have successfully added an Office Add-in.

To make use of Office functionality and styling for a given HTML page, add the following
references to the page's <head> section, adjusting relative paths as necessary:

    <!-- Office references: -->
    <link href="Content/Office.css" rel="stylesheet" type="text/css" />
    <script src="https://appsforoffice.microsoft.com/lib/1/hosted/office.js"></script>

    <!-- To enable offline debugging using a local reference to Office.js, use:                  -->
    <!--    <script src="Scripts/Office/MicrosoftAjax.js" type="text/javascript"></script>       -->
    <!--    <script src="Scripts/Office/1/office.js" type="text/javascript"></script>          -->


Note that the Office initialize function must be called before any JavaScript 
interaction with the Office API (once per page):

    Office.initialize = function (reason) {
        $(document).ready(function () {
            // Add initialization logic here.
        });
    };
