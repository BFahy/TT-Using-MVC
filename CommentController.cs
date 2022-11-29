using TypicalTools.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using DataAccess;

namespace TypicalTools.Controllers
{
    public class CommentController : Controller
    {
        private readonly DBContext context;

        public CommentController(DBContext dBContext)
        {
            context = dBContext;
        }

        [HttpGet]
        public IActionResult CommentList(int id)
        {
            List<Comment> comments = context.GetCommentsForProduct(id);

            if(comments == null)
            {
                return RedirectToAction("Index", "Product");
            }

            return View(comments);

        }


        // Show a form to add a new comment
        [HttpGet]
        public IActionResult AddComment(int productCode)
        {
            Comment comment = new Comment();
            comment.product_id = productCode;
            return View(comment);
        }

        // Receive and handle the newly created comment data
        [HttpPost]
        public IActionResult AddComment(Comment comment)
        {
            comment.session_id = HttpContext.Session.Id;
            comment.created_date = DateTime.Now;
            context.AddComment(comment);

            // A session id is only set once a value has been added!
            // adding a value here to ensure the session is created
            HttpContext.Session.SetString("CommentText", comment.text);

            return RedirectToAction("CommentList", "Comment", new { id = comment.product_id });
        }

        // Receive and handle a request to Delete a comment
        public IActionResult RemoveComment(int commentId)
        {
            var comment = context.GetSingleComment(commentId);

            // Check if the admin is logged in
            string authStatus = HttpContext.Session.GetString("Authenticated");
            bool isAdmin = !String.IsNullOrWhiteSpace(authStatus) && authStatus.Equals("True");

            // Peform the deletion conditionally
            if (comment.session_id == HttpContext.Session.Id || isAdmin)
            {
                context.DeleteComment(commentId);
            }

            return RedirectToAction("CommentList", "Comment", new {id = comment.product_id});
        }

        // Show a existing comment details in a form to allow for editing
        [HttpGet]
        public IActionResult EditComment(int commentId)
        {
            Comment comment = context.GetSingleComment(commentId);
            return View(comment);
        }

        // Receive and handle the edited comment data
        [HttpPost]
        public IActionResult EditComment(Comment comment)
        {
            if(comment == null)
            {
                return RedirectToAction("Index", "Product");
            }

            // Check if the admin is logged in
            string authStatus = HttpContext.Session.GetString("Authenticated");
            bool isAdmin = !String.IsNullOrWhiteSpace(authStatus) && authStatus.Equals("True");

            if (comment.session_id == HttpContext.Session.Id || isAdmin)
            {                
                context.EditComment(comment);
            }
            

            return RedirectToAction("CommentList", "Comment", new { id = comment.product_id });

        }
    }
}
