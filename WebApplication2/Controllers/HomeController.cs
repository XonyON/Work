using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        // GET: Index
        [Authorize]
        [HttpGet]
        public ActionResult Index(int id)
        {
            User user;
            using (DatabaseEntities2 context = new DatabaseEntities2())
            {
                user = context.Users.FirstOrDefault(i => i.Id == id);
            }


            return View(user);
        }

        [Authorize]
        public PartialViewResult ActiveItemList()
        {
            try
            {
                List<ToDoItem> items;
                using (DatabaseEntities2 context = new DatabaseEntities2())
                {
                    int myID = context.Users.First(x => x.login == HttpContext.User.Identity.Name).Id;
                    items = context.ToDoItems.Where(i => i.userID == myID && i.isComplete == false).ToList();
                }

                return PartialView("_Item", items);
            }
            catch
            {
                return PartialView("_Login_Layout");
            }
        }

        public PartialViewResult EndItemList()
        {
            List<ToDoItem> items;
            using (DatabaseEntities2 context = new DatabaseEntities2())
            {
                int myID = context.Users.First(x => x.login == HttpContext.User.Identity.Name).Id;
                items = context.ToDoItems.Where(i => i.userID == myID && i.isComplete == true).ToList();
            }

            return PartialView("_EndItem", items);
        }

        [HttpPost]
        public ActionResult AddItem(string Task, string date, bool priority)
        {
            try
            {
                string check = "";
                Nullable<System.DateTime> datetime;


                using (DatabaseEntities2 context = new DatabaseEntities2())
                {
                    int myID = context.Users.First(x => x.login == HttpContext.User.Identity.Name).Id;

                    if (Task != check)
                    {
                        if (date != "")
                            datetime = DateTime.Parse(date);
                        else datetime = null;

                        ToDoItem item = new ToDoItem(Task, datetime, priority, myID);
                        context.ToDoItems.Add(item);
                        context.SaveChanges();
                    }
                }

                return RedirectToAction("ActiveItemList");
            }
            catch
            {
                return RedirectToAction("ActiveItemList");
            }
        }

        public ActionResult Item()
        {
            return PartialView();
        }

        public ActionResult ShowTask()
        {
            bool check = bool.Parse(Session["showTask"] as string);
            if (check)
                Session["showTask"] = "false";
            else
                Session["showTask"] = "true";

            return RedirectToAction("EndItemList");
        }

        [HttpDelete]
        public void Delete(int id)
        {
            try
            {
                using (DatabaseEntities2 context = new DatabaseEntities2())
                {
                    context.ToDoItems.Remove(context.ToDoItems.First(itemId => itemId.Id == id));
                    context.SaveChanges();
                }
            }
            catch { return; }
        }

        [HttpPost]
        public PartialViewResult Details(int itemId)
        {
            try
            {
                ToDoItem item;
                using (DatabaseEntities2 context = new DatabaseEntities2())
                {
                    item = context.ToDoItems.First(i => i.Id == itemId);
                }

                return PartialView("_Details", item);
            }
            catch
            {
                return PartialView("_NotFound");
            }
        }

        [HttpPost]
        public JsonResult Complete(int id, bool check)
        {
            try
            {
                using (DatabaseEntities2 context = new DatabaseEntities2())
                {
                    if (check) context.ToDoItems.FirstOrDefault(i => i.Id == id).isComplete = false;
                    else
                    {
                        context.ToDoItems.FirstOrDefault(i => i.Id == id).isComplete = true;
                        foreach (var item in context.AddTasks.Where(i => i.itemID == id))
                        {
                            if (item!=null)
                            item.isComplete = true;
                        }
                    }
                    context.SaveChanges();
                }

                return Json(new { id = id, complete = !check });
            }
            catch
            {
                return Json(new { });
            }
        }

        [HttpPost]
        public void UnfinishTask(int id)
        {
            try
            {
                using (DatabaseEntities2 context = new DatabaseEntities2())
                {
                    context.ToDoItems.FirstOrDefault(i => i.Id == id).isComplete = false;
                    context.SaveChanges();
                }
            }
            catch
            {
                return;
            }
        }

        [HttpPost]
        public PartialViewResult FinishTask(int id)
        {
            try
            {
                using (DatabaseEntities2 context = new DatabaseEntities2())
                {
                    context.ToDoItems.FirstOrDefault(i => i.Id == id).isComplete = true;
                    context.SaveChanges();
                }

                return PartialView("_Item");
            }
            catch
            {
                return PartialView("_Item");
            }
        }

        [HttpPost]
        public JsonResult Mark(int id, bool check)
        {
            try
            {
                using (DatabaseEntities2 context = new DatabaseEntities2())
                {
                    if (check) context.ToDoItems.FirstOrDefault(i => i.Id == id).Mark = false;
                    else context.ToDoItems.FirstOrDefault(i => i.Id == id).Mark = true;
                    context.SaveChanges();
                }

                return Json(new { marked = !check });
            }
            catch { return Json(new { marked = check }); }
        }

        [HttpPost]
        public PartialViewResult AddInerTask(int itemid, string tasktext)
        {
            List<AddTask> tasks;
            AddTask item;

            using (DatabaseEntities2 context = new DatabaseEntities2())
            {
                item = new AddTask(itemid, tasktext);
                context.AddTasks.Add(item);
                context.SaveChanges();

                tasks = context.AddTasks.Where(i => i.itemID == itemid).ToList();
            }

            if (tasks.Count > 0) return PartialView("AddTasks", tasks);
            else return PartialView("AddTasks");
        }

        [HttpPost]
        public PartialViewResult ShowInerTask(int? taskid)
        {
            try
            {
                List<AddTask> tasks;
                using (DatabaseEntities2 context = new DatabaseEntities2())
                {
                    tasks = context.AddTasks.Where(i => i.itemID == taskid).ToList();
                }
                if (tasks.Count > 0) return PartialView("AddTasks", tasks);
                else return PartialView("AddTasks");
            }

            catch
            {
                return PartialView("AddTasks");
            }

        }

        [HttpPost]
        public void NoteEdit(int itemid, string notext)
        {
            try
            {
                using (DatabaseEntities2 context = new DatabaseEntities2())
                {
                    context.ToDoItems.First(i => i.Id == itemid).Note = notext;
                    context.SaveChanges();
                }
            }
            catch { return; }
        }

        [HttpPost]
        public void CompleteInerTask(string taskid, bool completed)
        {
            int id;

            try
            {
                id = int.Parse(taskid);
                using (DatabaseEntities2 context = new DatabaseEntities2())
                {
                    if (completed) context.AddTasks.First(i => i.Id == id).isComplete = false;
                    else context.AddTasks.First(i => i.Id == id).isComplete = true;
                    context.SaveChanges();
                }
            }
            catch
            {
                return;
            }
        }

        [HttpPost]
        public void DeleteInerTask(int id)
        {
                using (DatabaseEntities2 context = new DatabaseEntities2())
                {
                    context.AddTasks.Remove(context.AddTasks.First(i => i.Id == id));
                    context.SaveChanges();
                }
        }

        [HttpPost]
        public void FinishTime(int itemid, string time)
        {
            try
            {
                using (DatabaseEntities2 context = new DatabaseEntities2())
                {
                    if (time != "") context.ToDoItems.First(i => i.Id == itemid).FinishTime = DateTime.Parse(time);
                    else context.ToDoItems.First(i => i.Id == itemid).FinishTime = null;
                    context.SaveChanges();
                }
            }
            catch
            {
                return;
            }

        }

        [HttpPost]
        public void RingTime(int itemid, string time)
        {
            using (DatabaseEntities2 context = new DatabaseEntities2())
            {
                if (time != "") context.ToDoItems.First(i => i.Id == itemid).RememberTime = DateTime.Parse(time);
                else context.ToDoItems.First(i => i.Id == itemid).RememberTime = null;
                context.SaveChanges();
            }
        }
        
        public ActionResult Files(int id)
        {
            List<Models.File> files = null;
            using (DatabaseEntities2 context = new DatabaseEntities2())
            {
                files = context.Files.Where(f => f.itemid == id).ToList();
            }

            return PartialView("Files", files);
        }

        [HttpGet]
        public ActionResult GetFile(int? id)
        {
            using(DatabaseEntities2 context = new DatabaseEntities2())
            {
                Models.File temp = context.Files.First(f => f.Id == id);
                return File(temp.file1, temp.type);
            }
        }

        [HttpPost]
        public ActionResult UploadFile(int id)
        {
            using (DatabaseEntities2 context = new DatabaseEntities2())
            {
                HttpPostedFileBase file = Request.Files["upl"];
                //bool isUploaded = false;
                string message = "File upload failed";

                int length = file.ContentLength;
                string name = file.FileName;
                string type = file.ContentType;
                try
                {
                    using (BinaryReader reader = new BinaryReader(file.InputStream))
                    {
                        context.Files.Add(new Models.File(id, reader.ReadBytes(file.ContentLength), name, type, DateTime.Now));
                        context.SaveChanges();
                    }
                    //isUploaded = true;
                    message = "File uploaded successfully!";
                }
                catch (Exception ex)
                {
                    message = string.Format("File upload failed: {0}", ex.Message);
                }

                return RedirectToAction("Files", new { id = id });
            }
        }

        [HttpPost]
        public void DeleteFile(int id)
        {
            try
            {
                using (DatabaseEntities2 context = new DatabaseEntities2())
                {
                    context.Files.Remove(context.Files.First(i => i.Id == id));
                    context.SaveChanges();
                }
            }
            catch { return; }
        }

        public ActionResult Comments(int id)
        {
            List<Comment> comments;

            using (DatabaseEntities2 context = new DatabaseEntities2())
            {
                comments = context.Comments.Where(i => i.itemid == id).ToList();
            }

            return PartialView("Comments", comments);
        }

        //[HttpPost]
        //public ActionResult AddComment(int itemid, string text)
        //{
        //    string username = User.Identity.Name;
        //    using (DatabaseEntities2 context = new DatabaseEntities2())
        //    {
        //        context.Comments.Add(new Comment(username, text, itemid));
        //        context.SaveChanges();
        //    }

        //    return RedirectToAction("Comments", new { id = itemid });
        //}

        //[HttpDelete]
        //public void DeleteComment(int id)
        //{
        //    using (DatabaseEntities2 context = new DatabaseEntities2())
        //    {
        //        context.Comments.Remove(context.Comments.First(i => i.Id == id));
        //        context.SaveChanges();
        //    }
        //}
    }
}