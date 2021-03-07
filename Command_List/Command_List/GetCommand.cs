using System.Collections.Generic;
using Command_List.Commands;
using Command_List.Commands.SD_CMD;
using Classes;

namespace Command_List
{
    public static class GetCommand
    {
        public static IReadOnlyList<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            commands.Add(new MyAccess_Command());           //access
            commands.Add(new Add_Admin_Command());          //add_adm
            commands.Add(new Add_User_Command());           //add
            commands.Add(new Answer_Command());             //answer
            commands.Add(new Calculator_Command());         //calculator
            commands.Add(new Clear_Command());              //clear
            commands.Add(new GetCommands_Command());        //command, help
            commands.Add(new Delete_User_Command());        //delete
            commands.Add(new Message_Command());            //message
            commands.Add(new Name_Command());               //name
            commands.Add(new Params_Command());             //params
            commands.Add(new Points_Command());             //points
            commands.Add(new Quest_Command());              //quest
            commands.Add(new Quest_Start_Command());        //quest_start
            commands.Add(new ReAccess_Admin_Command());     //reaccess
            commands.Add(new Registration_Command());       //register
            commands.Add(new Reload_Command());             //reload
            commands.Add(new Remove_Command());             //remove
            commands.Add(new Spawn_Command());              //spam
            commands.Add(new Strart_Command());             //start
            commands.Add(new Time_Command());               //time
            commands.Add(new Translator_Command());         //translate
            commands.Add(new Triger_Command());             //triger
            commands.Add(new Undo_Command());               //undo
            commands.Add(new Update_Command());             //update
            commands.Add(new View_All_Admins_Command());    //view

            commands.Add(new Delete_Admin_Command());       //delete_adm

            return commands.AsReadOnly();
        }
    }
}
