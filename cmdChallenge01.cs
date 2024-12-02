using Autodesk.Revit.DB;
using System.Xml.Linq;

namespace RevitAddInBootCamp
{
    [Transaction(TransactionMode.Manual)]
    public class cmdChallenge01 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Your Module 01 Challenge code goes here

            //create a transaction to lock the model
            Transaction t = new Transaction(doc);
            t.Start("FIZZ BUZZ Challenge");

                        
            int numFloors = 250; // Declare a number variable and set it to 250
            double startingElevation = 0; // Declare a starting elevation variable and set it to 0
            double floorHeight = 15; // Declare a floor height variable and set it to 15
            int fizzCount = 0;
            int buzzCount = 0;
            int fizzbuzzCount = 0;


            //create a new level every 15ft
            // Loop through the number 1 to the number variable

            // Create a level for each number

            // After creating the level, increment the current elevation by the floor height value.


            for (int i = 1; i <= numFloors; i++)
            {
                Level newLevel = Level.Create(doc, i * floorHeight);
                newLevel.Name = $"level {i}";

                // create a filtered element collector to get view family type
                FilteredElementCollector collector1 = new FilteredElementCollector(doc);
                collector1.OfClass(typeof(ViewFamilyType));

                ViewFamilyType floorPlanVFT = null;
                foreach (ViewFamilyType curVFT in collector1)

                    if (curVFT.ViewFamily == ViewFamily.FloorPlan)
                    {
                        floorPlanVFT = curVFT;
                    }

                ViewFamilyType ceilingPlanVFT = null;
                foreach (ViewFamilyType curVFT in collector1)

                    if (curVFT.ViewFamily == ViewFamily.CeilingPlan)
                    {
                        ceilingPlanVFT = curVFT;
                    }

                // If the number is divisible by both 3 and 5, create a sheet and name it "FIZZBUZZ_#"

                if (i % 3 == 0 && i % 5 == 0)
                {
                    // get a title block
                    FilteredElementCollector collector2 = new FilteredElementCollector(doc);
                    collector2.OfCategory(BuiltInCategory.OST_TitleBlocks);
                    collector2.WhereElementIsElementType();

                    //create a view port
                    // create points
                    XYZ insPoint = new XYZ(1.85, 1, 0);
                    XYZ insPoint2 = new XYZ(1.5, 1, 0);
                    // create floor plan views
                    ViewPlan newFloorPlan = ViewPlan.Create(doc, floorPlanVFT.Id, newLevel.Id);
                    newFloorPlan.Name = newFloorPlan.Name = "FIZZBUZZ_#" + " " + i;
                    
                    // create sheets
                    ViewSheet newSheet = ViewSheet.Create(doc, collector2.FirstElementId());

                    newSheet.Name = $"FIZZBUZZ_#";
                    newSheet.SheetNumber = $"B{i}";
                    fizzbuzzCount++;

                    // add views to a sheet
                    Viewport newViewport = Viewport.Create(doc, newSheet.Id, newFloorPlan.Id, insPoint2);
                    
                }

                // If the number is divisible by 3, create a floor plan and name it "FIZZ_#"

                else if (i % 3 == 0)
                {

                    // create floor plan views
                    ViewPlan newFloorPlan = ViewPlan.Create(doc, floorPlanVFT.Id, newLevel.Id);
                    newFloorPlan.Name = newFloorPlan.Name = "FIZZ_#" + " " + i;
                    fizzCount++;

                }

                // If the number is divisible by 5, create a ceiling plan and name it "BUZZ_#"

                else if (i % 5 == 0)
                {
                    // create ceiling plan views
                    ViewPlan newCeilingPlan = ViewPlan.Create(doc, ceilingPlanVFT.Id, newLevel.Id);
                    newCeilingPlan.Name = $"BUZZ_{i}";
                    buzzCount++;
                }

            }
         

            t.Commit();
            t.Dispose();

            // alert user
            TaskDialog.Show("Complete", $"Created {numFloors} levels" + " " +
                $"Created {fizzCount} FIZZ views" + " " +
                $"Created {buzzCount} BUZZ views" + " " +
                $"Created {fizzbuzzCount} FIZZBUZZ sheets");


            return Result.Succeeded;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnChallenge01";
            string buttonTitle = "Module\r01";

            Common.ButtonDataClass myButtonData = new Common.ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Module01,
                Properties.Resources.Module01,
                "Module 01 Challenge");

            return myButtonData.Data;
        }
    }

}
