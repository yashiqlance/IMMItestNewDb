namespace IMMIWeb.Infrastructure
{
    public class EmailTemplateMaker
    {
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        public EmailTemplateMaker(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public  string GetTemplate(string action, string templateName, string[] param)
        {
             
            string FileInString = string.Empty;
            string webRootPath = _hostingEnvironment.WebRootPath;
            string contentRootPath = _hostingEnvironment.ContentRootPath + "StaticFiles\\";
            string filePath = Path.Combine(contentRootPath, templateName);

            bool directoryExists = System.IO.File.Exists(filePath);
            
            if (directoryExists)
            {
                FileInString = System.IO.File.ReadAllText(filePath);

                if (action == "sendEmailVerificationLink")
                {
                    FileInString = FileInString.Replace("$$$imagePath$$$", "https://guardian-temp.s3.us-east-2.amazonaws.com/uploads/image_2023_07_10T07_08_33_590Z.png");
                    FileInString = FileInString.Replace("$$$verificationLink$$$", param[1]);                    
                }
                else if (action == "welcomeUser")
                {
                    FileInString = FileInString.Replace("$$$userName$$$", param[0]);                    
                }
                else if (action == "EmailOTPVerification")
                {
                    FileInString = FileInString.Replace("$$$otp$$$", param[0]);
                    FileInString = FileInString.Replace("imagepath", "https://guardian-temp.s3.us-east-2.amazonaws.com/uploads/image_2023_07_10T07_08_33_590Z.png");
                }
                else if(action == "sendContactUsMail")
                {
                    FileInString = FileInString.Replace("$$$userName$$$", param[0]);
                    FileInString = FileInString.Replace("$$$description$$$", param[1]);
                    FileInString = FileInString.Replace("$$$email$$$", param[2]);
                    FileInString = FileInString.Replace("$$$subject$$$", param[3]);
                }
                else if (action == "ConsultantStripe")
                {
                    FileInString = FileInString.Replace("$$$stripelink$$$", param[0]);                    
                }
            }

            return FileInString;
        }
    }
}
