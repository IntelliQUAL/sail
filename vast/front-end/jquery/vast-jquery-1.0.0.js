/*
    Created:        12/18/2016
    Author:         David J. McKee
    Purpose:        This script is an implementation of IntelliQUAL VAST Design Patter for jQuery

    ************ Most Popular Functions ************:
    **** CRUD / NEW / SEARCH **** 
    vastNew(vastInfo, successFunction, failureFunction);                                                    // Creates an empty "new" instance of a given entity
    vastCreate(vastInfo, vastEntity, successFunction, failureFunction);                                     // Creates a new entity.
    vastRead(vastInfo, successFunction, failureFunction);                                                   // Reads an existing entity
    vastUpdate(vastInfo, vastEntity, successFunction, failureFunction);                                     // Updates an existing entity
    vastDelete(vastInfo, successFunction, failureFunction);                                                 // Deletes a single row by primary key
    vastSearch(vastInfo, successFunction, failureFunction);                                                 // Search for entity rows.
    
    **** HELPER FUNCTIONS ****    
    vastSetFieldValue(newOrReadResponse, fieldId, fieldValue);                                              // Sets any field value;
    vastReadRowFieldString(columndId, row, searchResponse);                                                 // Reads a single column value from search response row
    vastReadFieldString(newOrReadResponse, fieldId);                                                        // Reads a single field value from an entity    
    vastSetAllEntityValuesByID(newOrReadResponse);                                                          // Copy everything from a document to an entity     
    vastSaveField(vastInfo, fieldIdToUpdate, newValue, transactionId, successFunction, failureFunction);    // Updates a single field.
    vastClone(vastInfo);                                                                                    // Clones any data type    
    vastSetAllDocumentValuesByID(newOrReadResponse);                                                        // Copy everyting from an entity to the document.
    vastSetAllDocumentValuesByIdFromRow(row, searchResponse);                                               // Copy everything from a row to the document.            
    vastFillSelect(selectId, searchResponse, valueFieldId, displayNameFieldId, defaultValue);               // Fill and Select / Drop-down from any search response.
    vastEnsureFieldValue(vastInfo, newOrReadResponse, fieldId, fieldValue);                                 // Ensures that the given field exists.
    vastRemoveField(vastInfo, newOrReadResponse, fieldId);                                                  // Permanently removes a given field.
    vastNewEntity(vastInfo, fieldIdToUpdate, newValue, transactionId);                                      // Creates an entity with one field. Mainly used for custom operations.
    vastSafeSetFieldValue(newOrReadResponse, fieldId, fieldValue);                                          // Sets or adds a field to any entity without module interaction.

    ***** SEARCHING *****
    vastWhereEqualTo(fieldName, fieldValue, page, rowsPerPage);                                             // Search where fieldName equal to fieldValue  
    vastWhereEqualToOrderByAsc(fieldName, fieldValue, orderByfieldName, page, rowsPerPage);                 // Search where fieldName equal to fieldValue  
    vastWhereOr(fieldName, fieldValue1, fieldValue1, page, rowsPerPage);                                    // Search where fieldName equal to fieldValue1 or fieldValue2
    vastWhereAnd(fieldName1, fieldValue1, fieldName2, fieldValue2, page, rowsPerPage);                      // Search where fieldName equal to fieldValue1 or fieldValue2
    
    ***** ACCOUNT / AUTHENTICATION / AUTHORIZATION *****
    vastInitCreateAccount(vastInfo, successFunction, failureFunction);                                      // Creates a new account entity which must be filled in.
    vastCreateAccount(vastInfo, newAccountEntity, successFunction, failureFunction);                        // Creates a new account based on the entity from vastInitCreateAccount
    vastReadAccount(vastInfo, successFunction, failureFunction);                                            // Reads an existing account.
    vastListAccountTypes(vastInfo, successFunction, failureFunction);                                       // List all account types on the given domain.

    ***** METADATA *****
    vastGetFieldSchema(columnId, newOrReadResponse)                                                         // Read the column schema from a new or read response.
    vastGetColumnSchema(columnId, searchResponse);                                                          // Read the column schema from a search response.
*/

var QS_ACCOUNT_CODE = "a"
var QS_ACCOUNT_STATUS = "s";  
var QS_MODULE_ID = "m";
var QS_FORM_ID = "f";
var QS_PRIMARY_KEY = "pk";
var QS_ACTION_TYPE = "t";

var COOKIE_ACCOUNT_CODE = "AccountCode";
var COOKIE_AUTH_TOKEN = "AuthToken";
var COOKIE_USERNAME = "UserName";
var COOKIE_ACCOUNT_TYPE = "AccountType";

function vastInitialize(document, successFunction, failureFunction) {

    var vastInfo = {
        moduleId: getParameterByName(QS_MODULE_ID),
        formId: getParameterByName(QS_FORM_ID),
        primaryKeyFieldId: '',
        primaryKeyValue: getParameterByName(QS_PRIMARY_KEY),
        domain: DOMAIN_NAME,
        accountCode: getParameterByName(QS_ACCOUNT_CODE),
        accountType: '',
        accountStatus: getParameterByName(QS_ACCOUNT_STATUS),        
        actionType: getParameterByName(QS_ACTION_TYPE),        
        customDataPipeline: '',
        customOperationList: '',
        searchCriteria: null,
        authToken: jQuery.cookie(COOKIE_AUTH_TOKEN)
    }
    
    if (isNullOrWhiteSpace(vastInfo.accountCode)) {
        vastInfo.accountCode = jQuery.cookie(COOKIE_ACCOUNT_CODE);
    }
    
    var accountStatus = getParameterByName(QS_ACCOUNT_STATUS);

    if (isNullOrWhiteSpace(vastInfo.accountStatus)) {
        vastInfo.accountStatus = ACCOUNT_STATUS;
    }
    
    if (isNullOrWhiteSpace(vastInfo.accountType)) {
        vastInfo.accountType = jQuery.cookie(COOKIE_ACCOUNT_TYPE);
    }

    if (isVastAuthenticated(vastInfo)) {
        successFunction(vastInfo);
    } else {
        var authenticationFailed = true;
        failureFunction(authenticationFailed);
    }     
}

function vastNew(vastInfo, successFunction, failureFunction) {

    var url = GetDefaultHost() + '/v7/json/IQ.BUS.Vast.Entities/new/?' + buildStandardParams(vastInfo);

    $.ajax({
        url: url,
        dataType: "jsonp",
        success: successFunction,
        failure: failureFunction
    });
}

function vastCreate(vastInfo, vastEntity, successFunction, failureFunction) {

    vastInfo.actionType = 'Create';

    vastSave(vastInfo, vastEntity, successFunction, failureFunction, false, 0);    
}

function vastRead(vastInfo, successFunction, failureFunction) {

    vastInfo.actionType = "Read";

    var url = GetDefaultHost() + "/v7/json/IQ.BUS.Vast.Entities/" + vastInfo.primaryKeyValue + "/?" + buildStandardParams(vastInfo);

    $.ajax({
        url: url,
        dataType: "jsonp",
        success: successFunction,
        failure: failureFunction
    });

}

function vastUpdate(vastInfo, vastEntity, successFunction, failureFunction) {

    vastInfo.actionType = 'Update';

    vastSave(vastInfo, vastEntity, successFunction, failureFunction, true, 0);

}

function vastDelete(vastInfo, successFunction, failureFunction) {

    vastInfo.actionType = "Delete";

    var url = GetDefaultHost() + "/v7/json/IQ.BUS.Vast.Entities/" + vastInfo.primaryKeyValue + "/?cors-method=DELETE&" + buildStandardParams(vastInfo);

    $.ajax({
        url: url,
        dataType: "jsonp",
        success: successFunction,
        failure: failureFunction
    });

}

function vastSearch(vastInfo, successFunction, failureFunction) {

    vastInfo.actionType = "Search";

    var url = GetDefaultHost() + "/v7/json/IQ.BUS.Vast.Entities/?" + buildStandardParams(vastInfo);

    if (vastInfo.searchCriteria != null) {
        var jsonString = JSON.stringify(vastInfo.searchCriteria);
        var dataToSend = encodeURIComponent(jsonString);
        url = url + "&" + dataToSend;
    }

    $.ajax({
        url: url,
        dataType: "jsonp",
        success: successFunction,
        failure: failureFunction
    });

}

// Creates a copy of an object
function vastClone(any) {

    var response = null;

    var jsonString = JSON.stringify(any);

    response = JSON.parse(jsonString);

    return response;
}

// Clone an existing vastInfo so it can be reused.
function vastCloneInfo(vastInfo) {

    /*
    var vastInfoClone = {
        domain: vastInfo.domain,
        accountCode: vastInfo.accountCode,
        accountStatus: vastInfo.accountStatus,
        moduleId: vastInfo.moduleId,
        formId: vastInfo.formId,
        actionType: vastInfo.actionType,
        primaryKeyFieldId: vastInfo.primaryKeyFieldId,
        primaryKeyValue: vastInfo.primaryKeyValue,
        customDataPipeline: vastInfo.customDataPipeline,
        customOperationList: vastInfo.customOperationList,
        searchCriteria: vastInfo.searchCriteria,
        authToken: vastInfo.authToken
    }
    */

    var vastInfoClone = vastClone(vastInfo);

    return vastInfoClone;
}

// ******************************************* BEGIN: SEARCH HELPERS *******************************************

function vastWhereEqualTo(fieldName, fieldValue, page, rowsPerPage) {

    var SEARCH_CRITERIA = '{"Page":"","RowsPerPage":"","Where": {"Predicate": {"FieldName": "","OperatorType": "", "Value": ""}}}';

    var searchCriteria = JSON.parse(SEARCH_CRITERIA);

    searchCriteria.Page = page;
    searchCriteria.RowsPerPage = rowsPerPage;
    searchCriteria.Where.Predicate.FieldName = fieldName;
    searchCriteria.Where.Predicate.OperatorType = "EqualTo";
    searchCriteria.Where.Predicate.Value = fieldValue;

    return searchCriteria;
}

function vastWhereEqualToOrderByAsc(fieldName, fieldValue, orderByfieldName, page, rowsPerPage) {

    var SEARCH_CRITERIA = '{"Page":"","RowsPerPage":"","OrderBy": {"SortColumn": "","SortOrder": "Asc"},"Where": {"Predicate": {"FieldName": "","OperatorType": "", "Value": ""}}}';

    var searchCriteria = JSON.parse(SEARCH_CRITERIA);

    searchCriteria.Page = page;
    searchCriteria.RowsPerPage = rowsPerPage;
    searchCriteria.Where.Predicate.FieldName = fieldName;
    searchCriteria.Where.Predicate.OperatorType = "EqualTo";
    searchCriteria.Where.Predicate.Value = fieldValue;
    searchCriteria.OrderBy.SortColumn = orderByfieldName;

    return searchCriteria;

}

function vastWhereOr(fieldName, fieldValue1, fieldValue2, page, rowsPerPage) {

    var SEARCH_CRITERIA = '{"Page":"","RowsPerPage":"","Where": {"Predicate": {"FieldName": "","OperatorType": "EqualTo"},"Or": {"Predicate": {"FieldName": "","OperatorType": "EqualTo","Value": ""}}}}';

    var searchCriteria = JSON.parse(SEARCH_CRITERIA);

    searchCriteria.Page = page;
    searchCriteria.RowsPerPage = rowsPerPage;
    searchCriteria.Where.Predicate.FieldName = fieldName;
    searchCriteria.Where.Predicate.OperatorType = "EqualTo";
    searchCriteria.Where.Predicate.Value = fieldValue1;
    searchCriteria.Where.Or.Predicate.FieldName = fieldName;
    searchCriteria.Where.Or.Predicate.OperatorType = "EqualTo";
    searchCriteria.Where.Or.Predicate.Value = fieldValue2;

    return searchCriteria;
}

function vastWhereAnd(fieldName1, fieldValue1, fieldName2, fieldValue2, page, rowsPerPage) {

    var SEARCH_CRITERIA = '{"Page":"","RowsPerPage":"","Where": {"Predicate": {"FieldName": "","OperatorType": "EqualTo"},"And": {"Predicate": {"FieldName": "","OperatorType": "EqualTo","Value": ""}}}}';

    var searchCriteria = JSON.parse(SEARCH_CRITERIA);

    searchCriteria.Page = page;
    searchCriteria.RowsPerPage = rowsPerPage;
    searchCriteria.Where.Predicate.FieldName = fieldName1;
    searchCriteria.Where.Predicate.OperatorType = "EqualTo";
    searchCriteria.Where.Predicate.Value = fieldValue1;
    searchCriteria.Where.And.Predicate.FieldName = fieldName2;
    searchCriteria.Where.And.Predicate.OperatorType = "EqualTo";
    searchCriteria.Where.And.Predicate.Value = fieldValue2;

    return searchCriteria;

}

// ******************************************* END: SEARCH HELPERS *******************************************

// ******************************************* BEGIN: AUTHENTICATION AND AUTHORIZATION *******************************************
function vastAuthenticateUser(userName, password, vastInfo, successFunction, failureFunction) {

    var authRequest = {
        AccountCode: vastInfo.accountCode,
        AccountStatus: vastInfo.accountStatus,
        DomainName: vastInfo.domain,
        Password: password,
        Username: userName
    };

    var url = GetDefaultHost() + "/v7/json/IQ.BUS.Vast.AuthenticateUser/?" + buildStandardParams(vastInfo);

    var requestJson = JSON.stringify(authRequest);

    var dataToSend = encodeURIComponent(requestJson);
    url = url + "&" + dataToSend;
    
    $.ajax({
        url: url,
        dataType: "jsonp",
        success: function (authResponse) {

            if (isNullOrWhiteSpace(authResponse.AuthToken) == false) {
                jQuery.cookie(COOKIE_AUTH_TOKEN, authResponse.AuthToken);
                jQuery.cookie(COOKIE_ACCOUNT_CODE, vastInfo.accountCode);
                jQuery.cookie(COOKIE_USERNAME, userName);

                successFunction(authResponse, true);
            }
            else {
                successFunction(authResponse, false);
            }
        },
        failure: failureFunction
    });

}

function isVastAuthenticated(vastInfo) {

    var isAuthenticated = false;

    var authToken = vastInfo.authToken;

    if (isNullOrWhiteSpace(authToken)) {
        authToken = jQuery.cookie(COOKIE_AUTH_TOKEN);
    }

    isAuthenticated = !isNullOrWhiteSpace(authToken);

    return isAuthenticated;

}

function vastAuthenticateAuthToken(vastInfo, successFunction, failureFunction) {
    
}

function vastInitCreateAccount(vastInfo, successFunction, failureFunction) {

	vastInfo = vastCloneInfo(vastInfo);

	vastInfo.actionType = "New";
	
    var url = GetDefaultHost() + '/v7/json/IQ.BUS.Vast.Accounts/new/?' + buildStandardParams(vastInfo);

    $.ajax({
        url: url,
        dataType: "jsonp",
        success: successFunction,
        failure: failureFunction
    });

}

var CREATE_ACCOUNT_CREATE = 'CreateAccountCreate';

function vastCreateAccount(vastInfo, newAccountEntity, successFunction, failureFunction) {

	vastInfo = vastCloneInfo(vastInfo);
	
	vastInfo.actionType = "Create";
    vastInfo.customDataPipeline = CREATE_ACCOUNT_CREATE;
	vastInfo.customOperationList = "IQ.BUS.Vast.Common.AppendRequestTableSchema,IQ.BUS.Vast.Common.SetStandardFieldValues,IQ.OPS.Formatter.RemoveInvalidChars,IQ.OPS.New.EnforceUniqueConstraint," +
									"IQ.BUS.Vast.Common.RemoveDataNotInSchema,IQ.OPS.Create.CreateInDB,IQ.OPS.Instance.CreateAccount,IQ.OPS.Network.SMTP";
	vastInfo.authToken = ''; 	// Clear because a new one should be created.
	vastInfo.accountCode = '';	// Clear because it should be set on the result.
	
    var accountType = vastReadFieldString(newAccountEntity, "AccountType");
	var accountCode = vastReadFieldString(newAccountEntity, "AccountCode");
	
    vastCreate(vastInfo, newAccountEntity, function (creatAccountResponse) {
        
        vastInfo.authToken = creatAccountResponse.AuthToken;

        jQuery.cookie(COOKIE_ACCOUNT_CODE, accountCode);        
        jQuery.cookie(COOKIE_ACCOUNT_TYPE, accountType);
		jQuery.cookie(COOKIE_AUTH_TOKEN, vastInfo.authToken);

        successFunction(creatAccountResponse);

    }, failureFunction);
}

function vastReadAccount(vastInfo, successFunction, failureFunction) {
    
    if (isNullOrWhiteSpace(vastInfo.authToken)) {
        var url = GetDefaultHost() + "/v7/json/IQ.BUS.Vast.Accounts/" + vastInfo.accountCode + "/?" + buildStandardParams(vastInfo);

        $.ajax({
            url: url,
            dataType: "jsonp",
            success: successFunction,
            failure: failureFunction
        });
    }
    else {
        vastInfo = vastCloneInfo(vastInfo);
        vastInfo.moduleId = "MasterRepository";
        vastInfo.formId = "Account";

        var SEARCH_CRITERIA = '{"Page":"","RowsPerPage":"","Where": {"Predicate": {"FieldName": "","OperatorType": "", "Value": ""}}}';

        vastInfo.searchCriteria = JSON.parse(SEARCH_CRITERIA);
        vastInfo.searchCriteria.Page = 1;
        vastInfo.searchCriteria.RowsPerPage = 1;
        //vastInfo.searchCriteria.Where.Predicate.FieldName = "AccountCode";
        //vastInfo.searchCriteria.Where.Predicate.OperatorType = "EqualTo";
        //vastInfo.searchCriteria.Where.Predicate.Value = vastInfo.accountCode;

        vastSearch(vastInfo, function(accountList) {            
            if ((accountList.RowList == null) || (accountList.RowList.length == 0)) {
                vastNew(vastInfo, successFunction, failureFunction);
            } else {
                for (var rowIndex = 0; rowIndex < accountList.RowList.length; rowIndex++) {
                    var row = accountList.RowList[rowIndex];
                    var accountId = vastReadRowFieldString("AccountID", row, accountList);

                    vastInfo.primaryKeyFieldId = "AccountID";
                    vastInfo.primaryKeyValue = accountId;

                    vastRead(vastInfo, successFunction, failureFunction);
                }
            }

        }, failureFunction);        
    }
}

function vastListAccountTypes(vastInfo, successFunction, failureFunction) {

    vastInfo = vastCloneInfo(vastInfo);

    vastInfo.customOperationList = "IQ.OPS.Instance.ListAccountTypes";

    vastSearch(vastInfo, successFunction, failureFunction);

}

// ******************************************* END: AUTHENTICATION AND AUTHORIZATION *******************************************

function vastSaveField(vastInfo, fieldIdToUpdate, newValue, transactionId, successFunction, failureFunction) {

    vastSaveFieldInternal(vastInfo, fieldIdToUpdate, newValue, transactionId, successFunction, failureFunction, false);

}

function vastNewEntity(vastInfo, fieldIdToUpdate, newValue, transactionId) {

    var minRequest = '{"FieldGroupList": [{"FieldRowList": [{"FieldList": [{"ID": "ID","Value": ""},{"ID": "ID","Value": ""}]}]}],"ImmediateSubmission": false, "FormID": ""}';

    var singleFieldRequest = JSON.parse(minRequest);

    singleFieldRequest.FormID = vastInfo.formId;    
    singleFieldRequest.TransactionID = transactionId;
    singleFieldRequest.FieldGroupList[0].FieldRowList[0].FieldList[0].ID = vastInfo.primaryKeyFieldId;
    singleFieldRequest.FieldGroupList[0].FieldRowList[0].FieldList[0].Value = vastInfo.primaryKeyValue;
    singleFieldRequest.FieldGroupList[0].FieldRowList[0].FieldList[0].isDirty = true;
    singleFieldRequest.FieldGroupList[0].FieldRowList[0].FieldList[1].ID = fieldIdToUpdate;
    singleFieldRequest.FieldGroupList[0].FieldRowList[0].FieldList[1].Value = newValue;
    singleFieldRequest.FieldGroupList[0].FieldRowList[0].FieldList[1].isDirty = true;

    return singleFieldRequest;
}

function vastSafeSetFieldValue(newOrReadResponse, fieldId, fieldValue) {

    if (vastSetFieldValue(newOrReadResponse, fieldId, fieldValue) == false) {
        var field = '{"ID": "","Value": ""}';
        
        var newField = JSON.parse(field);

        newField.ID = fieldId;
        newField.Value = fieldValue;
        newField.isDirty = true;

        newOrReadResponse.FieldGroupList[0].FieldRowList[0].FieldList.push(newField);
    }

}

// We only allow updating one field at a time to the back-end
function vastSaveFieldInternal(vastInfo, fieldIdToUpdate, newValue, transactionId, successFunction, failureFunction, immediateSubmission) {

    var singleFieldRequest = vastNewEntity(vastInfo, fieldIdToUpdate, newValue, transactionId);
        
    singleFieldRequest.ImmediateSubmission = immediateSubmission;

    var dataToSend = encodeURIComponent(JSON.stringify(singleFieldRequest));

    var httpMethod = "POST";

    if (vastInfo.actionType == "Update") {
        httpMethod = "PUT";
    }
    
    // Remove extra data on immediateSubmission
    if (immediateSubmission) {
        vastInfo = vastCloneInfo(vastInfo);
        vastInfo.customOperationList = '';
    }
        
    var fullUrl = GetDefaultHost() + "/v7/json/IQ.BUS.Vast.Entities/?cors-method=" + httpMethod + "&" + buildStandardParams(vastInfo) + "&" + dataToSend;

    if (vastInfo.customDataPipeline == CREATE_ACCOUNT_CREATE) {
        fullUrl = GetDefaultHost() + "/v7/json/IQ.BUS.Vast.Accounts/?cors-method=" + httpMethod + "&" + buildStandardParams(vastInfo) + "&" + dataToSend;
    }

    $.ajax({
        url: fullUrl,
        dataType: "jsonp",
        success: successFunction,
        failure: failureFunction
    });

}

//******* HELPER FUNCTIONS BEYOND THIS POINT *******
// Important: Ensure .actionType is correct for metadata.
function vastEnsureFieldValue(vastInfo, newOrReadResponse, fieldId, fieldValue) {

    if (vastSetFieldValue(newOrReadResponse, fieldId, fieldValue) == false) {
        var vastInfoSaveField = vastCloneInfo(vastInfo);

        vastInfoSaveField.customOperationList = "IQ.OPS.Form.EnsureFormField";

        vastSaveField(vastInfoSaveField, "FieldID", fieldId, newOrReadResponse.TransactionID, function () { }, function () { }, false);

        var displayName = fieldId;
        var displaySequence = 0;

        // Make sure the column is visisble based on meta-data
        var fieldSchema = vastGetFieldSchema(fieldId, newOrReadResponse);

        if (isNullOrWhiteSpace(fieldSchema) == false) {

            if (isNullOrWhiteSpace(fieldSchema.DisplayName) == false) {
                displayName = fieldSchema.DisplayName;
            }

            if (isNullOrWhiteSpace(fieldSchema.DisplaySequence) == false) {
                displaySequence = fieldSchema.DisplaySequence;
            }
        }

        vastMetadataSetColumnVisibility(vastInfoSaveField, fieldId, vastInfo.actionType, 0, displayName, displaySequence);
    }

}

function vastRemoveField(vastInfo, newOrReadResponse, fieldId) {

    // TODO: FUTURE
    // var vastInfoRemoveField = vastCloneInfo(vastInfo);
    // vastInfoRemoveField.customOperationList = "IQ.OPS.Form.RemoveFormField";
    // vastSaveField(vastInfoRemoveField, "FieldID", fieldId, newOrReadResponse.TransactionID, function () { }, function () { }, false);
}

function vastSetFieldValue(newOrReadResponse, fieldId, fieldValue) {

    var found = false;
    
    if (newOrReadResponse != null) {
        if (newOrReadResponse.FieldGroupList != null) {
            newOrReadResponse.FieldGroupList.forEach(function (fieldGroup) {
                fieldGroup.FieldRowList.forEach(function (fieldRow) {
                    fieldRow.FieldList.forEach(function (field) {

                        if (field.ID === fieldId) {
                            found = true;

                            if ((isNullOrWhiteSpace(field.Value)) && (isNullOrWhiteSpace(fieldValue))) {
                                field.Value = ''; // Do nothing
                            } else {
                                // Only update the field if the value changed.
                                if (field.Value != fieldValue) {
                                    // Update value
                                    field.Value = fieldValue;
                                    field.isDirty = true;
                                }
                            }
                        }
                    });
                });
            });
        }
    }

    return found;
}

// Reads a string from a single entity value.
function vastReadFieldString(newOrReadResponse, fieldId) {

    result = '';

    if (newOrReadResponse != null) {
        if (newOrReadResponse.FieldGroupList != null) {
            newOrReadResponse.FieldGroupList.forEach(function (fieldGroup) {
                fieldGroup.FieldRowList.forEach(function (fieldRow) {
                    fieldRow.FieldList.forEach(function (field) {
                        if (field.ID === fieldId) {
                            result = field.Value;
                        }
                    });
                });
            });
        }
    }

    return result;
}

// Reads a string from a multi-entity (SearchResponse) value.
function vastReadRowFieldString(columndId, row, searchResponse) {

    var ordinalPosition = ordinalPositionFromID(columndId, searchResponse);
        
    return row[ordinalPosition];

}

function ordinalPositionFromID(columnId, searchResponse) {

    var ordinalPosition = 0;

    if (searchResponse.FieldSchemaList != null) {
        for (var columnIndex = 0; columnIndex < searchResponse.FieldSchemaList.length; columnIndex++) {

            var column = searchResponse.FieldSchemaList[columnIndex];

            if (column.ID === columnId) {
                ordinalPosition = column.OrdinalPosition;
                break;
            }
        }
    }

    return ordinalPosition;
}

function buildStandardParams(vastInfo) {
        
    if (isNullOrWhiteSpace(vastInfo.customDataPipeline)) {
        vastInfo.customDataPipeline = '';
    }

    if (isNullOrWhiteSpace(vastInfo.customOperationList)) {
        vastInfo.customOperationList = '';
    }
	
	var params = "d=" + DOMAIN_NAME + "&a=" + vastInfo.accountCode + "&n=" + vastInfo.authToken + "&" + QS_MODULE_ID + "=" + vastInfo.moduleId +
                    "&s=" + ACCOUNT_STATUS + "&f=" + vastInfo.formId + "&c=" + vastInfo.customDataPipeline + "&o=" + encodeURIComponent(vastInfo.customOperationList); // + "&callback=JSON_CALLBACK";

    return params;
}

function readFieldBool(columnGroupList, fieldId) {
				
    result = false;
				
	columnGroupList.forEach(function(columnGroup) {					
	        columnGroup.FieldRowList.forEach(function(fieldRow) {				
		        fieldRow.FieldList.forEach(function(field) {						
			    if (field.ID === fieldId) {							
			        result = (field.Value === "True");
			    }				
		    });
	    });				
	});
			
    return result;	
}

/********************** FILL HELPERS ****************************/
// SET ALL FORM VALUES FROM A NEW OR READ WHERE THE HTML INPUT ID MATCHES THE FIELD ID
function vastSetAllDocumentValuesByID(newOrReadResponse) {

    if (newOrReadResponse != null) {
        if (newOrReadResponse.FieldGroupList != null) {

            newOrReadResponse.FieldGroupList.forEach(function (fieldGroup) {
                fieldGroup.FieldRowList.forEach(function (fieldRow) {
                    fieldRow.FieldList.forEach(function (field) {

                        var element = $("#" + field.ID);
                        var isInput = element.is("input");

                        if (isInput) {
                            element.val(field.Value);
                            console.log('"#' + field.ID + '" set to "' + field.Value + '" with a default value of "' + field.Schema.DefaultValue + '"');
                        } else {
                            console.log('DOC DOES NOT CONTAIN: "#' + field.ID + '" source value "' + field.Value + '" with a default value of "' + field.Schema.DefaultValue + '"');
                        }
                    });
                });
            });

        }
    }

}

// SET ALL FORM VALUES FROM A SEARCH RESPONSE ROW THE HTML INPUT ID MATCHES THE FIELD ID
function vastSetAllDocumentValuesByIdFromRow(row, searchResponse) {

    if (searchResponse.FieldSchemaList != null) {
        for (var columnIndex = 0; columnIndex < searchResponse.FieldSchemaList.length; columnIndex++) {

            var column = searchResponse.FieldSchemaList[columnIndex];
            
            var element = $("#" + column.ID);
            var isInput = element.is("input");
            
            var ordinalPosition = ordinalPositionFromID(column.ID, searchResponse);

            var columnValue = row[ordinalPosition];

            if (isInput) {
                element.val(columnValue);
                console.log('"#' + field.ID + '" set to "' + columnValue + '" with a default value of "' + column.DefaultValue + '"');
            } else {
                console.log('DOC DOES NOT CONTAIN: "#' + column.ID + '" source value "' + columnValue + '" with a default value of "' + column.DefaultValue + '"');
            }
        }
    }

}

// SET ALL ENTITY VALUES FROM THE DOCUMENT WHERE THE ELEMENT ID MATCHES THE ENTITY FIELD ID
function vastSetAllEntityValuesByID(newOrReadResponse) {

    var elements = document.body.getElementsByTagName("*");

    for (var elementIndex = 0; elementIndex < elements.length; elementIndex++) {
     
        var elementId = elements[elementIndex].id;

        if (isNullOrWhiteSpace(elementId) == false) {
            var newFieldValue = '';

            var jqueryElement = $('#' + elementId);

            var elementValue = jqueryElement.val();
            var elementNodeName = jqueryElement.prop("nodeName");
            var elementType = jqueryElement.prop("type");

            if (isNullOrWhiteSpace(elementNodeName)) {
                elementNodeName = "";                
            } else {
                elementNodeName = elementNodeName.toLowerCase();
            }

            if (isNullOrWhiteSpace(elementType)) {
                elementType = "";
            } else {
                elementType = elementType.toLowerCase();
            }
                        
            switch (elementNodeName) {
                case "input":
                    switch (elementType) {
                        case "checkbox":
                            newFieldValue = jqueryElement.prop('checked');
                            
                            if (newFieldValue) {
                                newFieldValue = 'True';
                            } else {
                                newFieldValue = 'False';
                            }
                            break;
                            
                        default:
                            if (isNullOrWhiteSpace(elementValue) == false) {
                                newFieldValue = elementValue;
                            } else {
                                var elementText = jqueryElement.text();

                                if (isNullOrWhiteSpace(elementText) == false) {
                                    newFieldValue = elementText;
                                }
                            }
                            break;
                    }

                    break;
                    
                default:
                    if (isNullOrWhiteSpace(elementValue) == false) {
                        newFieldValue = elementValue;
                    } else {
                        var elementText = jqueryElement.text();

                        if (isNullOrWhiteSpace(elementText) == false) {
                            newFieldValue = elementText;
                        }
                    }
                    break;
            }
            
            vastSetFieldValue(newOrReadResponse, elementId, newFieldValue);
        }
    }
}

// FILL SELECT / DROP-DOWN FROM SEARCH RESPONSE
function vastFillSelect(selectId, searchResponse, valueFieldId, displayNameFieldId, defaultValue) {

    var optionsHtml = '';

    for (var rowIndex = 0; rowIndex < searchResponse.RowList.length; rowIndex++) {

        var row = searchResponse.RowList[rowIndex];

        var optionValue = vastReadRowFieldString(valueFieldId, row, searchResponse);
        var optionDisplayName = vastReadRowFieldString(displayNameFieldId, row, searchResponse);

        if (optionValue === defaultValue) {
            optionsHtml += '<option value="' + optionValue + '" selected="selected">' + optionDisplayName + '</option>';
        } else {
            optionsHtml += '<option value="' + optionValue + '">' + optionDisplayName + '</option>';
        }
    }

    // Success
    $('#' + selectId).html(optionsHtml);

}

// WARNING: Recursive logic to save all form field values one at a time to allow for jsonp / QueryString length limitations.
function vastSave(vastInfo, vastEntity, successFunction, failureFunction, isUpdate, saveCount) {

    var done = false;
    var submitted = false;
    var breakOut = false;

    // We need to recursively save until all fields have been saved.
    while ((done == false) && (submitted == false)) {

        for (var fieldGroupIndex = 0; fieldGroupIndex < vastEntity.FieldGroupList.length; fieldGroupIndex++) {

            var fieldGroup = vastEntity.FieldGroupList[fieldGroupIndex];

            for (var fieldRowIndex = 0; fieldRowIndex < fieldGroup.FieldRowList.length; fieldRowIndex++) {

                var fieldRow = fieldGroup.FieldRowList[fieldRowIndex];

                for (var fieldIndex = 0; fieldIndex < fieldRow.FieldList.length; fieldIndex++) {

                    var field = fieldRow.FieldList[fieldIndex];

                    if (field.isDirty) {
                        console.log('"#' + field.ID + '" with value "' + field.Value + '" HAS changed and is being saved."');
                        submitted = true;
                        breakOut = true;
                        saveCount++;

                        vastSaveFieldInternal(vastInfo, field.ID, field.Value, vastEntity.TransactionID, function () {

                            vastSave(vastInfo, vastEntity, successFunction, failureFunction, isUpdate, saveCount);

                            // Success
                        }, failureFunction, true, isUpdate);
                    }
                    else {
                        console.log('"#' + field.ID + '" with value "' + field.Value + '" has not changed and is being deleted."');
                    }

                    // Remove this single field.
                    fieldRow.FieldList.splice(fieldIndex, 1);
                    break;
                }

                // Check to see if the parent contains zero rows
                if (fieldRow.FieldList.length == 0) {
                    fieldGroup.FieldRowList.splice(fieldRowIndex, 1);
                    break;
                }

                if (breakOut) {
                    break;
                }
            }

            if (fieldGroup.FieldRowList.length == 0) {
                vastEntity.FieldGroupList.splice(fieldGroupIndex, 1);
                break;
            }

            if (breakOut) {
                break;
            }
        }

        done = (vastEntity.FieldGroupList.length == 0);
    }

    if ((submitted == false) && (done == true)) {
        if (saveCount > 0) {
            // Execute the final submission.
            vastSaveFieldInternal(vastInfo, '', '', vastEntity.TransactionID, successFunction, failureFunction, false, isUpdate);
        } else {
            successFunction(vastEntity);
        }
    }
}


/*************************** BEGIN METADATA ***************************/
function vastGetFieldSchema(fieldId, newOrReadResponse) {

    var schema = null;
    
    if (newOrReadResponse != null) {
        if (newOrReadResponse.FieldGroupList != null) {
            newOrReadResponse.FieldGroupList.forEach(function (fieldGroup) {
                fieldGroup.FieldRowList.forEach(function (fieldRow) {
                    fieldRow.FieldList.forEach(function (field) {
                        if (field.ID === fieldId) {
                            schema = field.Schema;
                        }
                    });
                });
            });
        }
    }

    return schema;

}

function vastGetColumnSchema(columnId, searchResponse) {

    var schema = null;

    if (searchResponse.FieldSchemaList != null) {
        for (var columnIndex = 0; columnIndex < searchResponse.FieldSchemaList.length; columnIndex++) {
            var columnSchema = searchResponse.FieldSchemaList[columnIndex];

            if (columnSchema.ID === columnId) {
                schema = columnSchema;
                break;
            }
        }
    }

    return schema;
}

function vastMetadataSetColumnVisibility(vastInfo, fieldId, action, internal, displayName, displaySequence) {

    var vastInfoSetColumnVisibility = vastClone(vastInfo);

    vastInfoSetColumnVisibility.moduleId = vastInfoSetColumnVisibility.moduleId + "Metadata";
    vastInfoSetColumnVisibility.formId = vastInfoSetColumnVisibility.formId + "ColumnVisibility";

    vastInfoSetColumnVisibility.searchCriteria = vastWhereAnd("ID", fieldId, "Action", action, 1, 1);

    vastSearch(vastInfoSetColumnVisibility, function (columnVisibilitySearchResult) {

        if (columnVisibilitySearchResult.RowList != null) {
            if (columnVisibilitySearchResult.RowList.length > 0) {
                // If the date exists, then check for changes.
                for (var rowIndex = 0; rowIndex < columnVisibilitySearchResult.RowList.length; rowIndex++) {
                    var row = columnVisibilitySearchResult.RowList[rowIndex];
                    // We expect only one row.
                    var internalCurrent = vastReadRowFieldString("Internal", row, columnVisibilitySearchResult);
                    var displayNameCurrent = vastReadRowFieldString("DisplayName", row, columnVisibilitySearchResult);
                    var displaySequenceCurrent = vastReadRowFieldString("DisplaySequence", row, columnVisibilitySearchResult);

                    if ((internal.toString() != internalCurrent) || (displayName != displayNameCurrent) || (displaySequence != displaySequenceCurrent)) {
                        alert("Invalid Metadata for " + vastInfoSetColumnVisibility.moduleId + ", " + vastInfoSetColumnVisibility.formId + ", and " + fieldId);
                        // Metadata tables do no currently have primary keys and therefore cannot be updated.
                    } else {
                        //successFunction();
                    }
                }
            } else {
                // FUTURE: if the data does not exists, then create it.
                //successFunction();
            }
        } else {
            // FUTURE: if the data does not exists, then create it.
            //successFunction();
        }
    }, function () { });   
}

/*************************** BEGIN METADATA ***************************/

