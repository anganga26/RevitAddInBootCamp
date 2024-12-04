using System.Xml.Linq;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.Exceptions;
using System.Windows.Controls;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;


namespace RevitAddInBootCamp
{
    [Transaction(TransactionMode.Manual)]
    public class cmdChallenge02 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Your Module 02 Challenge code goes here

            {
                // Prompt user to select elements
                TaskDialog.Show("Select lines", "Select some lines to convert to Revit elements.");
                IList<Reference> selectedReferences = uidoc.Selection.PickObjects(ObjectType.Element, "Select model lines");

                // 1. prompt user to select elements
                /*TaskDialog.Show("Select lines", "Select some lines to convert to Revit elements.");
                List<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select some elements").ToList();*/

                // Filter for model curves
                List<ModelCurve> modelCurves = new List<ModelCurve>();
                foreach (Reference reference in selectedReferences)
                {
                    Element elem = doc.GetElement(reference);
                    if (elem is ModelCurve modelCurve)
                    {
                        modelCurves.Add(modelCurve);
                    }
                }

                using (Transaction t = new Transaction(doc, "Generate Elements from Model Lines"))
                {
                    t.Start();

                    foreach (ModelCurve modelCurve in modelCurves)
                    {
                        Curve curve = modelCurve.GeometryCurve;
                        GraphicsStyle lineStyle = modelCurve.LineStyle as GraphicsStyle;
                        
                            if (lineStyle != null)
                        {
                            switch (lineStyle.Name)
                            {
                                case "A-GLAZ":
                                    CreateStorefrontWall(doc, curve);
                                    break;
                                case "A-WALL":
                                    CreateGenericWall(doc, curve);
                                    break;
                                case "M-DUCT":
                                    CreateDuct(doc, curve);
                                    break;
                                case "P-PIPE":
                                    CreatePipe(doc, curve);
                                    break;
                                default:
                                    // Do nothing for unrecognized line styles
                                    break;
                            }
                        }
                    }

                    t.Commit();
                }
                                             
                void CreateStorefrontWall(Document doc, Curve curve)
                {
                    // Get the Storefront wall type
                    WallType storefrontType = new FilteredElementCollector(doc)
                        .OfClass(typeof(WallType))
                        .Cast<WallType>()
                        .FirstOrDefault(w => w.Name.Contains("Storefront"));

                    if (storefrontType != null)
                    {
                        Wall.Create(doc, curve, storefrontType.Id, doc.ActiveView.GenLevel.Id, 20, 0, false, false);
                    }
                }

                void CreateGenericWall(Document doc, Curve curve)
                {
                    // Get the Generic 8" wall type
                    WallType genericWallType = new FilteredElementCollector(doc)
                        .OfClass(typeof(WallType))
                        .Cast<WallType>()
                        .FirstOrDefault(w => w.Name.Contains("Generic") && w.Name.Contains("8\""));

                    if (genericWallType != null)
                    {
                        Wall.Create(doc, curve, genericWallType.Id, doc.ActiveView.GenLevel.Id, 20, 0, false, false);
                    }
                }

                void CreateDuct(Document doc, Curve curve)
                {
                    // Get system types
                    FilteredElementCollector systemCollector = new FilteredElementCollector(doc);
                    systemCollector.OfClass(typeof(MEPSystemType));

                    // Get duct system type
                    MEPSystemType ductSystem = GetSystemTypeByName(doc, "Supply Air");

                    // Get the default duct type
                    DuctType ductType = new FilteredElementCollector(doc)
                        .OfClass(typeof(DuctType))
                        .Cast<DuctType>()
                        .FirstOrDefault();                                                      
                                        
                    if (ductType != null)
                    {
                        Duct.Create(doc, ductSystem.Id, ductType.Id, doc.ActiveView.GenLevel.Id, curve.GetEndPoint(0), curve.GetEndPoint(1));
                        
                    }
                }

                void CreatePipe(Document doc, Curve curve)
                {
                   // Get system types
                    FilteredElementCollector systemCollector = new FilteredElementCollector(doc);
                    systemCollector.OfClass(typeof(MEPSystemType));

                    // Get pipe system type
                    MEPSystemType pipeSystem = GetSystemTypeByName(doc, "Domestic Hot Water");                                       
                    
                    // Get the default pipe type
                    PipeType pipeType = new FilteredElementCollector(doc)
                        .OfClass(typeof(PipeType))
                        .Cast<PipeType>()
                        .FirstOrDefault();

                    if (pipeType != null)
                    {
                        //Pipe.Create(doc, pipeType.Id, doc.ActiveView.GenLevel.Id, curve.GetEndPoint(0), curve.GetEndPoint(1));
                        Pipe.Create(doc, pipeSystem.Id, pipeType.Id, doc.ActiveView.GenLevel.Id, curve.GetEndPoint(0), curve.GetEndPoint(1));
                       
                    }
                }
            }

            return Result.Succeeded;
        }

        private MEPSystemType GetSystemTypeByName(Document doc, string v)

        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            collector.OfClass(typeof(MEPSystemType));

            foreach (MEPSystemType type in collector)

            {
                if (type.Name == v)

                    return type;
            }

            return null;

        }

        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnChallenge02";
            string buttonTitle = "Module\r02";

            Common.ButtonDataClass myButtonData = new Common.ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Module02,
                Properties.Resources.Module02,
                "Module 02 Challenge");

            return myButtonData.Data;
        }
    }

}
