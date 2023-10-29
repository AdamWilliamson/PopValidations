export function GetFiles(validatorName: string): Promise<
  [
    Validator: string,
    Request: string,
    OpenApi: string,
    Validation: string
  ]
> {
    return GetFilesBase(validatorName, "validations");
}

export function GetDemoFiles(validatorName: string): Promise<
  [
    Validator: string,
    Request: string,
    OpenApi: string,
    Validation: string
  ]
> {
    return GetFilesBase(validatorName, "demonstrations");
}

export function GetFilesBase(validatorName: string, folder: string): Promise<
  [
    Validator: string,
    Request: string,
    OpenApi: string,
    Validation: string
  ]
> {
  return Promise.all([
    fetch(`./${folder}/${validatorName}_Validator.txt`).then(
      async (content) => await content.text()
    ),
    fetch(`./${folder}/${validatorName}_Request.txt`).then(
      async (content) => await content.text()
    ),
    fetch(`./${folder}/${validatorName}_OpenApi.txt`).then(
      async (content) => await content.text()
    ),
    fetch(`./${folder}/${validatorName}_Validation.txt`).then(
      async (content) => await content.text()
    ),
  ]).then(
    ([validator_result, request_result, openapi_result, validation_result]) => {
      const new_validator_result = validator_result
        .split("\n")
        .map((str) => {
          return str.length &&
            str.charAt(0) == " " &&
            str.charAt(1) == " " &&
            str.charAt(2) == " " &&
            str.charAt(3) == " "
            ? str.substr(4)
            : str;
        })
        .join("\n");

        const new_request_result = request_result
        .split("\n")
        .map((str) => {
          return str.length &&
            str.charAt(0) == " " &&
            str.charAt(1) == " " &&
            str.charAt(2) == " " &&
            str.charAt(3) == " "
            ? str.substr(4)
            : str;
        })
        .join("\n");

      return [
        new_validator_result,
        new_request_result,
        openapi_result,
        validation_result,
      ];
    }
  );
}
