namespace Edelweiss.MVC.Controllers
{
    public class DefaultController : Controller
    {
        public void SetField(string fieldName, object value)
        {
            Model.Set(fieldName, value);
        }
    }
}