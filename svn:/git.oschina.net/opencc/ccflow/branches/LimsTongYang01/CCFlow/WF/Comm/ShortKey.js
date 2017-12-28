function DoKeyDown()
{
        if(event.keyCode==13)
        {
        return;
          event.keyCode=9;
          return ;
        }
		 
		   if ( (event.ctrlKey || event.altKey ) && String.fromCharCode(event.keyCode)=="S")
           {
              document.getElementById( "ToolBar1$Btn_Save").click();
              return false;
            }
            
           if ( (event.ctrlKey || event.altKey ) && String.fromCharCode(event.keyCode)=="L")
           {
              document.getElementById( "ToolBar1$Btn_Search").click();
              return false;
           }
            
             if ( (event.ctrlKey || event.altKey ) && String.fromCharCode(event.keyCode)=="P")
           {
              document.getElementById( "ToolBar1$Btn_P").click();
              return false;
            }
           
            
             if ( (event.ctrlKey || event.altKey ) && String.fromCharCode(event.keyCode)=="I")
             {
              document.getElementById( "ToolBar1$Btn_New").click();
               return false;
             }
             
             
             if ( (event.altKey ) && String.fromCharCode(event.keyCode)=="C")
             {
                document.getElementById( "ToolBar1$Btn_SaveAndClose").click();
                return false;
             }
             
             if ( (event.ctrlKey || event.altKey ) && String.fromCharCode(event.keyCode)=="R")
             {
                document.getElementById( "ToolBar1$Btn_SaveAndNew").click();
                return false;
             }
             
              if ( (event.ctrlKey || event.altKey ) && String.fromCharCode(event.keyCode)=="D")
             {
                document.getElementById( "ToolBar1$Btn_Delete").click();
                return false;
             }
		}