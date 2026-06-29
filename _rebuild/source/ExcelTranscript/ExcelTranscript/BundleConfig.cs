using System.Web.Optimization;

namespace ExcelTranscript
{
	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));
			bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include("~/Scripts/jquery-ui-{version}.js"));
			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));
			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js"));
			bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/bootstrap.css", "~/Content/site.css"));
			bundles.Add(new StyleBundle("~/Content/jQuery-File-Upload").Include("~/Content/jQuery.FileUpload/css/jquery.fileupload.css", "~/Content/jQuery.FileUpload/css/jquery.fileupload-ui.css", "~/Content/blueimp-gallery2/css/blueimp-gallery.css", "~/Content/blueimp-gallery2/css/blueimp-gallery-video.css", "~/Content/blueimp-gallery2/css/blueimp-gallery-indicator.css"));
			bundles.Add(new ScriptBundle("~/bundles/jQuery-File-Upload").Include("~/Scripts/jQuery.FileUpload/vendor/jquery.ui.widget.js", "~/Scripts/jQuery.FileUpload/tmpl.min.js", "~/Scripts/jQuery.FileUpload/load-image.all.min.js", "~/Scripts/jQuery.FileUpload/jquery.iframe-transport.js", "~/Scripts/jQuery.FileUpload/jquery.fileupload.js", "~/Scripts/jQuery.FileUpload/jquery.fileupload-process.js", "~/Scripts/jQuery.FileUpload/jquery.fileupload-image.js", "~/Scripts/jQuery.FileUpload/jquery.fileupload-audio.js", "~/Scripts/jQuery.FileUpload/jquery.fileupload-video.js", "~/Scripts/jQuery.FileUpload/jquery.fileupload-validate.js", "~/Scripts/jQuery.FileUpload/jquery.fileupload-ui.js", "~/Scripts/blueimp-gallery2/js/blueimp-gallery.js", "~/Scripts/blueimp-gallery2/js/blueimp-gallery-video.js", "~/Scripts/blueimp-gallery2/js/blueimp-gallery-indicator.js", "~/Scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js"));
		}
	}
}
