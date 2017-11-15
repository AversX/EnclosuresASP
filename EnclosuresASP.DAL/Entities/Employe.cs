namespace EnclosuresASP.DAL.Entities
{
    public class Employe//работник
    {
        public int EmployeID { get; set; }
        public string FullName { get; set; } //ФИО
        public virtual Position EmpPosition { get; set; } //должность
    }
}
