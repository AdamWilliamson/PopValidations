var fs = require('fs');

const validatorBasePath =   "../PopValidations.Demonstration_Tests/ValidatorDemoTests/";
const demoBasePath =        "../PopValidations.Demonstration_Tests/Examples/";
const validatorOutputBasePath = "public/validations/";
const demoOutputBasePath = "public/demonstrations/";

const beginIdent = "//Begin-";
const endIdent = "//End-";
const identValidator = "Validator";
const identRequest = "Request";

function GetValidationFilesFor(validator)
{
    return [
        { inputFile: validatorBasePath + `/${validator}/${validator}_DemoTest.cs`, ident: identValidator, outputFile: validatorOutputBasePath + `${validator}_Validator.txt` },
        { inputFile: validatorBasePath + `/${validator}/${validator}_DemoTest.cs`, ident: identRequest, outputFile: validatorOutputBasePath + `${validator}_Request.txt` },
        { inputFile: validatorBasePath + `/${validator}/${validator}_DemoTest.OpenApi.approved.json`, ident: null, outputFile: validatorOutputBasePath + `${validator}_OpenApi.txt` },
        { inputFile: validatorBasePath + `/${validator}/${validator}_DemoTest.Validation.approved.json`, ident: null, outputFile: validatorOutputBasePath + `${validator}_Validation.txt` }
    ];
}

function GetDemonstrationFilesFor(demonstration)
{
    return [
        { inputFile: demoBasePath + `/${demonstration}/${demonstration}Demonstration.cs`, ident: identValidator, outputFile: demoOutputBasePath + `${demonstration}_Validator.txt` },
        { inputFile: demoBasePath + `/${demonstration}/${demonstration}Demonstration.cs`, ident: identRequest, outputFile: demoOutputBasePath + `${demonstration}_Request.txt` },
        { inputFile: demoBasePath + `/${demonstration}/${demonstration}DemonstrationTests.OpenApi.approved.json`, ident: null, outputFile: demoOutputBasePath + `${demonstration}_OpenApi.txt` },
        { inputFile: demoBasePath + `/${demonstration}/${demonstration}DemonstrationTests.Validation.approved.json`, ident: null, outputFile: demoOutputBasePath + `${demonstration}_Validation.txt` }
    ];
}

const CodeToCopy = [
    ...GetValidationFilesFor("ForEach"),
    ...GetValidationFilesFor("Include"),
    ...GetValidationFilesFor("Is"),
    ...GetValidationFilesFor("IsEmpty"),
    ...GetValidationFilesFor("IsEnum"),
    ...GetValidationFilesFor("IsEqualTo"),
    ...GetValidationFilesFor("IsGreaterThan"),
    ...GetValidationFilesFor("IsGreaterThanOrEqualTo"),
    ...GetValidationFilesFor("IsLengthExclusivelyBetween"),
    ...GetValidationFilesFor("IsLengthInclusivelyBetween"),
    ...GetValidationFilesFor("IsLessThan"),
    ...GetValidationFilesFor("IsLessThanOrEqualTo"),
    ...GetValidationFilesFor("IsNotEmpty"),
    ...GetValidationFilesFor("IsNotNull"),
    ...GetValidationFilesFor("IsNull"),
    ...GetValidationFilesFor("Scope"),
    ...GetValidationFilesFor("ScopedData"),
    ...GetValidationFilesFor("ScopeWhen"),
    ...GetValidationFilesFor("SetValidator"),
    ...GetValidationFilesFor("Vitally"),
    ...GetValidationFilesFor("When"),
    ...GetDemonstrationFilesFor("Basic"),
    ...GetDemonstrationFilesFor("Moderate"),
    ...GetDemonstrationFilesFor("Advanced")
];

for(const copyItem of CodeToCopy){
    fs.readFile(copyItem.inputFile, 'utf8', function (err, data) {
        if (err) throw err;
        
        let fileDataToCopy = "";
        if (copyItem.ident != null){
            let firstItem = data.indexOf(beginIdent+copyItem.ident);
            let secondItem = data.indexOf(endIdent+copyItem.ident);
            
            
            if (firstItem >= 0 && secondItem > firstItem){
                //console.log(copyItem.inputFile + " > " + firstItem + ":" + secondItem);
                fileDataToCopy = data.substring(firstItem + beginIdent.length + copyItem.ident.length, secondItem)
            }
            else {
                throw copyItem.inputFile + " does not include a begining and end";
            }
        } else {
            //console.log(copyItem.inputFile + " > Copying Whole File");
            fileDataToCopy = data;
        }

        fs.writeFile(copyItem.outputFile, fileDataToCopy, function(err){
            //console.log(copyItem.inputFile + ": copying to :"  + copyItem.outputFile);
            if (err) throw err;
        });
    });
}

console.log("Successfully Completed");