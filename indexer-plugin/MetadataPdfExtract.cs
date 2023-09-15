///////////////////////////////////////////////////////////
// Plugin PdfExtractIndexationPlugin : file MetadataPdfExtract.cs
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Sinequa.Common;
using Sinequa.Configuration;
using Sinequa.Plugins;
using Sinequa.Connectors;
using Sinequa.Indexer;
using Sinequa.Search;
using Sinequa.Ml.Plugins;
using System.Web;
using System.Linq;
//using Sinequa.Ml;

namespace Sinequa.Plugin
{
	public class MetadataPdfExtract : IndexationPlugin{
		public override bool OnAfterAnalysis(IndexationDoc doc, Dictionary < string, string > columns){
			// Fetching the converted document Text
			string text = doc.GetValue("textconversion");

			// Getting FileName
			string fname = doc.FileName;

			// Checking if fname is European Patent document's FileName
			if(fname.Contains("EP")){
				// Setting Title
				columns["title"] = doc.ConversionTitle;
				Sys.Log2(20, "mapping --> title: " + doc.ConversionTitle);

				// Matching Claims section and getting Index(Location)
				string regexClaimText = @"[\n]Claims[\n\s\wA-Za-z0-9.,:()-;καβγδε']*";
				Match matchClaim = Regex.Match(text, regexClaimText);
				int claimStartLocation = matchClaim.Captures[0].Index;

				int claimEndLocation = Regex.Match(text, @"[\n]REFERENCES CITED IN THE DESCRIPTION").Captures[0].Index;

				// Fetching and modifying the Entity
					columns["entity9"] = modifyMyEntity(doc.GetValue("meta.entity9"),claimStartLocation,claimEndLocation); //ANATOMY
					columns["entity13"] = modifyMyEntity(doc.GetValue("meta.entity13"),claimStartLocation,claimEndLocation); //BIOCHEM
					columns["entity5"] = modifyMyEntity(doc.GetValue("meta.entity5"),claimStartLocation,claimEndLocation); //DRUG
					columns["entity15"] = modifyMyEntity(doc.GetValue("meta.entity15"),claimStartLocation,claimEndLocation); //GENE
					columns["entity6"] = modifyMyEntity(doc.GetValue("meta.entity6"),claimStartLocation,claimEndLocation); //INDICATION

				// Fetching Date Of Filing
				string dof="";
				Match matchDof = Regex.Match(text, @"\(22\)[a-zA-Z0-9.\-+_&:=""\/\s,<#>;Ã••'@]*: [a-zA-Z0-9.\-+_&=""\/\s,<#>;Ã••'@]*\(");
				if (matchDof.Success){
					string dofExtract = matchDof.Captures[0].Value;
			        string[] dofSplit= dofExtract.Split(':');
			        dof = dofSplit[1].Substring(0,dofSplit[1].Length-1);
					Sys.Log2(20, "Metadata Date of Filing (Matched): " + dof);
					columns["sourcestr10"] = dof;
					Sys.Log2(20, "mapping --> sourcestr10: " + dof);
				}else{
					Sys.Log2(20, "Metadata Date of Filing (Not Matched)");
				}

				// Fetching Application Number
				string appNum="";
				Match matchAppNum = Regex.Match(text, @"\(21\)[a-zA-Z0-9.\-+_&:=""\/\s,<#>;Ã••'@]*: [a-zA-Z0-9.\-+_&=""\/\s,<#>;Ã••'@]*\(");
				if (matchAppNum.Success){
					string appNumExtract = matchAppNum.Captures[0].Value;
			        string[] appNumSplit= appNumExtract.Split(':');
			        appNum = appNumSplit[1].Substring(0,appNumSplit[1].Length-1);
					Sys.Log2(20, "Metadata Application Number (Matched): " + appNum);
					columns["sourcestr11"] = appNum;
					Sys.Log2(20, "mapping --> sourcestr11: " + appNum);
				}else{
					Sys.Log2(20, "Metadata Application Number (Not Matched)");
				}
			}
			else if(fname.Contains("US")){
				//Fetching Document Title
				string usTitle="";
				Match matchAppNum = Regex.Match(text, @"\(54\)[a-zA-Z0-9.\-+_&:=""\/\s,<#>;Ã••'@]*");
				if (matchAppNum.Success){
					string usTitleExtract = matchAppNum.Captures[0].Value;
				    string[] usTitleSplit= usTitleExtract.Split(')');
				    usTitle = usTitleSplit[1].Substring(0,usTitleSplit[1].Length-1);
					columns["title"] = usTitle;
					Sys.Log2(20, "mapping --> title: " + usTitle);
				}

				// Matching Claims section and getting Claim Start Index(Location)
				string regexClaimStartText = @"[\n]What is claimed is:";
				string regexClaimStartText1 = @"[\n]The invention claimed is:";
				string regexClaimStartText2 = @"[\n]We claim:";

				if(Regex.Match(text, regexClaimStartText).Success){
					int claimLocation = Regex.Match(text, regexClaimStartText).Captures[0].Index;

					// Fetching and modifying the Entity
					columns["entity9"] = modifyMyEntity(doc.GetValue("meta.entity9"),claimLocation); //ANATOMY
					columns["entity13"] = modifyMyEntity(doc.GetValue("meta.entity13"),claimLocation); //BIOCHEM
					columns["entity5"] = modifyMyEntity(doc.GetValue("meta.entity5"),claimLocation); //DRUG
					columns["entity15"] = modifyMyEntity(doc.GetValue("meta.entity15"),claimLocation); //GENE
					columns["entity6"] = modifyMyEntity(doc.GetValue("meta.entity6"),claimLocation); //INDICATION
				}
				else if(Regex.Match(text, regexClaimStartText1).Success){
					int claimLocation1 = Regex.Match(text, regexClaimStartText1).Captures[0].Index;

					// Fetching and modifying the Entity
					columns["entity9"] = modifyMyEntity(doc.GetValue("meta.entity9"),claimLocation1); //ANATOMY
					columns["entity13"] = modifyMyEntity(doc.GetValue("meta.entity13"),claimLocation1); //BIOCHEM
					columns["entity5"] = modifyMyEntity(doc.GetValue("meta.entity5"),claimLocation1); //DRUG
					columns["entity15"] = modifyMyEntity(doc.GetValue("meta.entity15"),claimLocation1); //GENE
					columns["entity6"] = modifyMyEntity(doc.GetValue("meta.entity6"),claimLocation1); //INDICATION
				}
				else if(Regex.Match(text, regexClaimStartText2).Success){
					int claimLocation2 = Regex.Match(text, regexClaimStartText2).Captures[0].Index;

					// Fetching and modifying the Entity
					columns["entity9"] = modifyMyEntity(doc.GetValue("meta.entity9"),claimLocation2); //ANATOMY
					columns["entity13"] = modifyMyEntity(doc.GetValue("meta.entity13"),claimLocation2); //BIOCHEM
					columns["entity5"] = modifyMyEntity(doc.GetValue("meta.entity5"),claimLocation2); //DRUG
					columns["entity15"] = modifyMyEntity(doc.GetValue("meta.entity15"),claimLocation2); //GENE
					columns["entity6"] = modifyMyEntity(doc.GetValue("meta.entity6"),claimLocation2); //INDICATION
				}
			}
			else if(fname.Contains("WO")){
				//Fetching Document Title
				string usTitle="";
				Match matchAppNum = Regex.Match(text, @"\(54\)[a-zA-Z0-9.\-+_&:=""\/\s,<#>;Ã••'@\n]*");
				if (matchAppNum.Success){
					string usTitleExtract = matchAppNum.Captures[0].Value;
				    string[] usTitleSplit= usTitleExtract.Split(':');
				    usTitle = usTitleSplit[1].Substring(0,usTitleSplit[1].Length-1);
					columns["title"] = usTitle;
					Sys.Log2(20, "mapping --> title: " + usTitle);
				}

				// Matching Claims section and getting Claim Start Index(Location)
				string regexClaimStartText = @"[\n]What is claimed is:";
				string regexClaimStartText1 = @"[\n]WHAT IS CLAIMED IS";
				string regexClaimStartText2 = @"[\n]What is claimed herein is:";
				string regexClaimStartText3 = @"[\n]WHAT IS CLAIMED HEREIN IS";
				string regexClaimStartText4 = @"[\n]Claims:";
				string regexClaimStartText5 = @"[\n]CLAIMS";
				// string regexClaimStartText1 = @"[\n]The invention claimed is:";
				// string regexClaimStartText2 = @"[\n]We claim:";

				if(Regex.Match(text, regexClaimStartText).Success){
					int claimLocation = Regex.Match(text, regexClaimStartText).Captures[0].Index;

					// Fetching and modifying the Entity
					columns["entity9"] = modifyMyEntity(doc.GetValue("meta.entity9"),claimLocation); //ANATOMY
					columns["entity13"] = modifyMyEntity(doc.GetValue("meta.entity13"),claimLocation); //BIOCHEM
					columns["entity5"] = modifyMyEntity(doc.GetValue("meta.entity5"),claimLocation); //DRUG
					columns["entity15"] = modifyMyEntity(doc.GetValue("meta.entity15"),claimLocation); //GENE
					columns["entity6"] = modifyMyEntity(doc.GetValue("meta.entity6"),claimLocation); //INDICATION

					
					// return true;
				}
				else if(Regex.Match(text, regexClaimStartText1).Success){
					int claimLocation1 = Regex.Match(text, regexClaimStartText1).Captures[0].Index;

					// Fetching and modifying the Entity
					columns["entity9"] = modifyMyEntity(doc.GetValue("meta.entity9"),claimLocation1); //ANATOMY
					columns["entity13"] = modifyMyEntity(doc.GetValue("meta.entity13"),claimLocation1); //BIOCHEM
					columns["entity5"] = modifyMyEntity(doc.GetValue("meta.entity5"),claimLocation1); //DRUG
					columns["entity15"] = modifyMyEntity(doc.GetValue("meta.entity15"),claimLocation1); //GENE
					columns["entity6"] = modifyMyEntity(doc.GetValue("meta.entity6"),claimLocation1); //INDICATION

					
					// return true;
				}
				else if(Regex.Match(text, regexClaimStartText2).Success){
					int claimLocation2 = Regex.Match(text, regexClaimStartText2).Captures[0].Index;

					// Fetching and modifying the Entity
					columns["entity9"] = modifyMyEntity(doc.GetValue("meta.entity9"),claimLocation2); //ANATOMY
					columns["entity13"] = modifyMyEntity(doc.GetValue("meta.entity13"),claimLocation2); //BIOCHEM
					columns["entity5"] = modifyMyEntity(doc.GetValue("meta.entity5"),claimLocation2); //DRUG
					columns["entity15"] = modifyMyEntity(doc.GetValue("meta.entity15"),claimLocation2); //GENE
					columns["entity6"] = modifyMyEntity(doc.GetValue("meta.entity6"),claimLocation2); //INDICATION

					
					// return true;
				}
				else if(Regex.Match(text, regexClaimStartText3).Success){
					int claimLocation3 = Regex.Match(text, regexClaimStartText3).Captures[0].Index;

					// Fetching and modifying the Entity
					columns["entity9"] = modifyMyEntity(doc.GetValue("meta.entity9"),claimLocation3); //ANATOMY
					columns["entity13"] = modifyMyEntity(doc.GetValue("meta.entity13"),claimLocation3); //BIOCHEM
					columns["entity5"] = modifyMyEntity(doc.GetValue("meta.entity5"),claimLocation3); //DRUG
					columns["entity15"] = modifyMyEntity(doc.GetValue("meta.entity15"),claimLocation3); //GENE
					columns["entity6"] = modifyMyEntity(doc.GetValue("meta.entity6"),claimLocation3); //INDICATION

					
					// return true;
				}
				else if(Regex.Match(text, regexClaimStartText4).Success){
					int claimLocation4 = Regex.Match(text, regexClaimStartText4).Captures[0].Index;

					// Fetching and modifying the Entity
					columns["entity9"] = modifyMyEntity(doc.GetValue("meta.entity9"),claimLocation4); //ANATOMY
					columns["entity13"] = modifyMyEntity(doc.GetValue("meta.entity13"),claimLocation4); //BIOCHEM
					columns["entity5"] = modifyMyEntity(doc.GetValue("meta.entity5"),claimLocation4); //DRUG
					columns["entity15"] = modifyMyEntity(doc.GetValue("meta.entity15"),claimLocation4); //GENE
					columns["entity6"] = modifyMyEntity(doc.GetValue("meta.entity6"),claimLocation4); //INDICATION

					
					// return true;
				}
				else if(Regex.Match(text, regexClaimStartText5).Success){
					int claimLocation5 = Regex.Match(text, regexClaimStartText5).Captures[0].Index;

					// Fetching and modifying the Entity
					columns["entity9"] = modifyMyEntity(doc.GetValue("meta.entity9"),claimLocation5); //ANATOMY
					columns["entity13"] = modifyMyEntity(doc.GetValue("meta.entity13"),claimLocation5); //BIOCHEM
					columns["entity5"] = modifyMyEntity(doc.GetValue("meta.entity5"),claimLocation5); //DRUG
					columns["entity15"] = modifyMyEntity(doc.GetValue("meta.entity15"),claimLocation5); //GENE
					columns["entity6"] = modifyMyEntity(doc.GetValue("meta.entity6"),claimLocation5); //INDICATION

					
					// return true;
				}
				// else{
				// 	return false;
				// }
			}
			return true;
		}

		public static string modifyMyEntity(string entity, int location){
            string [] splitEntity = entity.Split(';');
            string finalEntityToReturn = "";
            string semiFinalEntity = "";
            Boolean flagSemiFinalEntityNull = true;

            foreach(string seperatedEntity in splitEntity.Reverse()){
                //Checking for if seperatedEntity contains ','
                if(seperatedEntity.Contains(",") && IsNumeric(seperatedEntity)){
                    //Split the seperatedEntity on ','
                    string [] commaSeperatedEntity = seperatedEntity.Split(',');
                    //Iterating through each commaSeperatedEntity
                    for(int i=0; i<commaSeperatedEntity.Length; i=i+2){
                        //Comparing commaSeperatedEntity with location
                        if(compareLocation(commaSeperatedEntity[i],location)){
                            //Recombining the commaSeperatedEntity after comparing
                            semiFinalEntity = semiFinalEntity + commaSeperatedEntity[i] + "," + commaSeperatedEntity[i+1] + ",";
                        }
                    }
                    //Checking if semiFinalEntity is null or ""
                    if(semiFinalEntity == ""){
                        //Setting flagSemiFinalEntityNull to 'true' to exclude the semiFinalEntity, seperatedEntity(Entity name and Entity code) in finalEntityToReturn
                        flagSemiFinalEntityNull = true;
                    }
                    else{
                        //Setting flagSemiFinalEntityNull to 'false' to exclude the semiFinalEntity, seperatedEntity(Entity name and Entity code) in finalEntityToReturn
                        flagSemiFinalEntityNull = false;
                        //Removing the extra ',' in the end
                        semiFinalEntity = semiFinalEntity.TrimEnd(',');
                        //Checking if the finalEntityToReturn is Empty
                        if(finalEntityToReturn == ""){
                            //Adding the first item (semiFinalEntity) to finalEntityToReturn
                            finalEntityToReturn =  semiFinalEntity;
                        }
                        else{
                            //Adding semiFinalEntity to the FinalEntityToReturn
                            finalEntityToReturn =  semiFinalEntity + ";" + finalEntityToReturn;
                        }
                        //Setting the semiFinalEntity to Empty
                        semiFinalEntity="";
                    }
                }
                else{
                    //Checking if flagSemiFinalEntityNull is false
                    if(!flagSemiFinalEntityNull){
                        //Adding seperatedEntity (Entity name and Entity code) in finalEntityToReturn for which semiFinalEntity is already added in finalEntityToReturn
                        finalEntityToReturn =  seperatedEntity + ";" + finalEntityToReturn;
                    }
                }
            }
            return finalEntityToReturn;
    	}

		public static Boolean compareLocation(string value, int location){
			//Convert string to Integer
			int entityLoc = Convert.ToInt32(value);

			//Initially setting flag to false
			Boolean flag = false;

			if(location <= entityLoc){
				flag = true;
			}
			else{
				flag = false;
			}
			return flag;
		}
    	public static bool IsNumeric(string text){
			double _out;
			return double.TryParse(text, out _out);
    	}

		//Override method modifyEntity for start and End Location integer
		public static string modifyMyEntity(string entity, int locationStart, int locationEnd){
            string [] splitEntity = entity.Split(';');
            string finalEntityToReturn = "";
            string semiFinalEntity = "";
            Boolean flagSemiFinalEntityNull = true;

            foreach(string seperatedEntity in splitEntity.Reverse()){
                //Checking for if seperatedEntity contains ','
                if(seperatedEntity.Contains(",") && IsNumeric(seperatedEntity)){
                    //Split the seperatedEntity on ','
                    string [] commaSeperatedEntity = seperatedEntity.Split(',');
                    //Iterating through each commaSeperatedEntity
                    for(int i=0; i<commaSeperatedEntity.Length; i=i+2){
                        //Comparing commaSeperatedEntity with location
                        if(compareLocation(commaSeperatedEntity[i],locationStart, locationEnd)){
                            //Recombining the commaSeperatedEntity after comparing
                            semiFinalEntity = semiFinalEntity + commaSeperatedEntity[i] + "," + commaSeperatedEntity[i+1] + ",";
                        }
                    }
                    //Checking if semiFinalEntity is null or ""
                    if(semiFinalEntity == ""){
                        //Setting flagSemiFinalEntityNull to 'true' to exclude the semiFinalEntity, seperatedEntity(Entity name and Entity code) in finalEntityToReturn
                        flagSemiFinalEntityNull = true;
                    }
                    else{
                        //Setting flagSemiFinalEntityNull to 'false' to exclude the semiFinalEntity, seperatedEntity(Entity name and Entity code) in finalEntityToReturn
                        flagSemiFinalEntityNull = false;
                        //Removing the extra ',' in the end
                        semiFinalEntity = semiFinalEntity.TrimEnd(',');
                        //Checking if the finalEntityToReturn is Empty
                        if(finalEntityToReturn == ""){
                            //Adding the first item (semiFinalEntity) to finalEntityToReturn
                            finalEntityToReturn =  semiFinalEntity;
                        }
                        else{
                            //Adding semiFinalEntity to the FinalEntityToReturn
                            finalEntityToReturn =  semiFinalEntity + ";" + finalEntityToReturn;
                        }
                        //Setting the semiFinalEntity to Empty
                        semiFinalEntity="";
                    }
                }
                else{
                    //Checking if flagSemiFinalEntityNull is false
                    if(!flagSemiFinalEntityNull){
                        //Adding seperatedEntity (Entity name and Entity code) in finalEntityToReturn for which semiFinalEntity is already added in finalEntityToReturn
                        finalEntityToReturn =  seperatedEntity + ";" + finalEntityToReturn;
                    }
                }
            }
            return finalEntityToReturn;
    	}
		
		//Override comparelocation with start and end location integer
		public static Boolean compareLocation(string value, int location1, int location2){
			//Convert string to Integer
			int entityLoc = Convert.ToInt32(value);

			//Initially setting flag to false
			Boolean flag = false;

			if((location1 <= entityLoc) && (entityLoc <= location2)){
				flag = true;
			}
			else{
				flag = false;
			}
			return flag;
		}
	}
}
