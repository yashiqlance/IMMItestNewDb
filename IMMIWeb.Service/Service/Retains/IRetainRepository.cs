using IMMIWeb.Service.Models;
using IMMIWeb.Service.Repo;
using Nancy.Routing.Trie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.Retains
{
    public interface IRetainRepository : IGenericRepository<IMMIWeb.Retain>
    {
        List<RetainConsultantDetails> RetainConsultantDetails(int ConsultantId);
        int AddRetainConsultant(Retain param);
        int AddRetainConsultantPayment(RetainPayment param);
        List<GetDocumentandPaymentDetails> GetDocumentandPaymentDetails(int UserId, int ConsultantId);
        int PayRetentionAmount(payretentionamountparam param);
        void AddUserDocuments(List<UserDocument> lstUserAddDocument, int uId, int cId);
        void AddEmiTable(List<Emitable> lstEmitable);
        bool RemoveUserDocument(string FileName, string FileExtension, int UserId);
        List<GetRetainConsultnatListForUserViewModel> GetRetainConsultnatListForUser(int Id);
        List<GetRetainConsultnatListForUserViewModel> GetRetainUserListForConsultnat(int Id);
    }
}
