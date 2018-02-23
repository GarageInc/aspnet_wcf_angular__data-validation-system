(function() {
    angular.module("network-services", [])
        .service("networkService", function($http) {
            var vm = this;
            vm.requestBuilder = function() {
                var request = {};
                this.method = function(method) {
                    request["method"] = method;
                    return this;
                }
                this.url = function(url) {
                    request["url"] = url;
                    return this;
                }
                this.body = function(body) {
                    request["data"] = body;
                    return this;
                }
                this.params = function(params) {
                    request["params"] = params;
                    return this;
                }
                this.build = function() {
                    return request;
                }
                return this;
            }
            vm.sendRequest = function(request) {
                return $http(request).then(function(response) {
                    return response.data;
                }, function(error) {
                    //console.log(error);
                    return error;
                });
            }

            return vm;
        })

        .service("crudService", function(networkService) {
            var vm = this;

            vm.list = function(controllerName) {
                var queryObj = networkService.requestBuilder().method("GET").url(buildUrl(controllerName, "List")).build();
                return networkService.sendRequest(queryObj);
            }
            vm.create = function(controllerName, record) {
                var queryObj = networkService.requestBuilder().method("POST").url(buildUrl(controllerName, "Create")).body(record).build();
                return networkService.sendRequest(queryObj);
            }
            vm.update = function(controllerName, record) {
                var queryObj = networkService.requestBuilder().method("POST").url(buildUrl(controllerName, "Update")).body(record).build();
                return networkService.sendRequest(queryObj);
            }
            vm.remove = function(controllerName, id) {
                var queryObj = networkService.requestBuilder().method("POST").url(buildUrl(controllerName, "Delete")).params({id : id}).build();
                return networkService.sendRequest(queryObj);
            }

            vm.get = function (controllerName, action, params) {
                return networkService.sendRequest(networkService.requestBuilder().method("GET").url(buildUrl(controllerName, action)).params(params).build());
            }

            vm.post = function (controllerName, action, params) {
                return networkService.sendRequest(networkService.requestBuilder().method("POST").url(buildUrl(controllerName, action)).body(params).build());
            }

            function buildUrl(controllerName, methodName) {
                return "/" + controllerName + "/" + methodName;
            }

            return vm;
        }).service("commonService", function ($timeout) {
            var vm = this;

            vm.SCALES = {
                seconds: "seconds",
                minutes: "minutes",
                hours: "hours",
                days: "days",
                months: "months"
            }

            vm.TYPES = {
                Value: {
                    Id: 1,
                    Name: "Value"
                },
                Length: {
                    Id: 2,
                    Name: "Length"
                },
                NumberOfDesctinctChanges: {
                    Id: 3,
                    Name: "NumberOfDesctinctChanges"
                },
                NumberOfChanges: {
                    Id: 4,
                    Name: "NumberOfChanges"
                },
                DataType: {
                    Id: 5,
                    Name: "DataType"
                }
            }

            vm.OPERATORS = {
                Between: {
                    Id: 13
                },
                NotBetween: {
                    Id: 14
                },
                Exists: {
                    Id: 17
                },
                NotExist: {
                    Id: 18
                },
                Empty: {
                    Id: 19
                },
                NotEmpty: {
                    Id: 20
                },
                ChangesPerValidationMoreThanInPercent: {
                    Id: 21
                },
                DifferenceBetweeenTherLastMoreThan: {
                    Id: 22
                },
                DifferenceBetweeenTherLastLessThan: {
                    Id: 23
                },
                DifferenceBetweeenTherLastEquals: {
                    Id: 24
                }
            }

            vm.buildNumberOfChanges = function (rule) {
                var numberOfChangesValue = {
                    numberOfChanges: rule.NumberOfChanges,
                    intervalInSeconds: rule.Value
                }
                if (numberOfChangesValue.intervalInSeconds < 60) {

                    numberOfChangesValue.intervalValue = numberOfChangesValue.intervalInSeconds;
                    numberOfChangesValue.scale = vm.SCALES.seconds;
                } else if (numberOfChangesValue.intervalInSeconds < 60 * 60) {

                    numberOfChangesValue.intervalValue = numberOfChangesValue.intervalInSeconds / 60;
                    numberOfChangesValue.scale = vm.SCALES.minutes;
                } else if (numberOfChangesValue.intervalInSeconds < 60 * 60 * 24) {

                    numberOfChangesValue.intervalValue = numberOfChangesValue.intervalInSeconds / (60 * 60)
                    numberOfChangesValue.scale = vm.SCALES.hours;
                } else if (numberOfChangesValue.intervalInSeconds < 60 * 60 * 24 * 30) {

                    numberOfChangesValue.intervalValue = numberOfChangesValue.intervalInSeconds / (60 * 60 * 24);
                    numberOfChangesValue.scale = vm.SCALES.days;
                } else if (numberOfChangesValue.intervalInSeconds < 60 * 60 * 24 * 30 * 12) {

                    numberOfChangesValue.intervalValue = numberOfChangesValue.intervalInSeconds / (60 * 60 * 24 * 30);
                    numberOfChangesValue.scale = vm.SCALES.months;
                } else {
                    //console.log("not implemented for year!");
                }

                return numberOfChangesValue;
            }

            vm.setRuleValueById = function (rule, value, type, dataTypeId, operatorId) {
                
            	if (typeof value === 'string') {
            		value = value.replace(new RegExp('&nbsp;', 'g'), ' ');
	            }

                rule.DataTypeId = 0;

                if (type == vm.TYPES.DataType.Name) {

                    rule.Value = type;
                    rule.DataTypeId = dataTypeId;
                } else if (type == vm.TYPES.NumberOfChanges.Name || type == vm.TYPES.NumberOfDesctinctChanges.Name) {

                    rule.Value = value.intervalInSeconds;// is seconds
                    rule.NumberOfChanges = value.numberOfChanges;
                } else if (type === vm.TYPES.Value.Name) {

                    if (operatorId === vm.OPERATORS.Exists.Id ||
                        operatorId === vm.OPERATORS.NotExist.Id ||
                        operatorId === vm.OPERATORS.Empty.Id ||
                        operatorId === vm.OPERATORS.NotEmpty.Id) {
                        rule.Value = "";

                    } else {

                        rule.Value = value;
                    }

                }
                else {

                    if (operatorId == vm.OPERATORS.Between.Id || operatorId == vm.OPERATORS.NotBetween.Id) {

                        rule.Value = value.first + "|" + value.second;
                    } else {

                        rule.Value = value;
                    }
                }
            }


            var setEditableBlockProperty = function (rule, $scope) {

                var blockRuleName = $("." + ($scope.isDisabledRules ? "suffix" : "") + "RuleName" + rule.Id);
                
                blockRuleName.editable("destroy");
                blockRuleName.editable({
                    disabled: $scope.isDisabledRules,
                    type: "text",
                    prepend: "not-selected",
                    inputclass: 'form-control',
                    placement: 'right',
                    display: function (value, sourceData) {

                        if (value && value != "") {

                            var is1or0 = value == "0" || value == "1";

                            var containsIdenticalOrParts = $scope.validationRules.filter(function (item) {
                                return (item.RuleName == value && (!rule.Id || item.Id !== rule.Id)) || item.RuleName !== value && (item.RuleName.indexOf(value) > -1
                                    || value.indexOf(item.RuleName) > -1);
                            });

                            var containsEmptySpaces = value.indexOf(' ') > -1;

                            if (is1or0 || containsEmptySpaces || (containsIdenticalOrParts && containsIdenticalOrParts.length > 0)) {

                                console.log(is1or0)
                                console.log(containsEmptySpaces)
                                console.log(containsIdenticalOrParts)
                                value = rule.RuleName;
                                toastr["error"]("RuleName '"+value+"'must be unique, not part of other names and without empty spaces.");
                            } else {

                                if ($scope.selectedValidationSetting.Setting.Expression) {
                                    $scope.selectedValidationSetting.Setting.Expression = $scope.selectedValidationSetting.Setting.Expression.replace(new RegExp(rule.RuleName, "g"), value);
                                } else {
                                    $scope.selectedValidationSetting.Setting.Expression = "";
                                }
                            }

                            rule.RuleName = value;
                            //$scope.$apply();

                            $(this).text(value).css("color", "green");
                        } else {
                            $(this).text("not-selected").css("color", "red");
                        }
                    }
                });

                var blockPropertyName = $("." + ($scope.isDisabledRules ? "suffix" : "") + "PropertyName" + rule.Id);

                blockPropertyName.editable("destroy");
                blockPropertyName.editable({
                    disabled: $scope.isDisabledRules,
                    prepend: "not-selected",
                    inputclass: 'form-control',
                    placement: 'right',
                    source: $scope.xmlNodes.map(function (node) {
                        return {
                            value: node.Id,
                            text: node.Path,
                        }
                    }),
                    display: function (value, sourceData) {

                        if (rule.Id < 0) {
                            rule.ParameterId = "";
                            rule.OperatorId = "";
                            rule.Value = "";
                        }// pass

                        var elem = $.grep(sourceData, function (o) {
                            return o.value == value;
                        });

                        if (elem.length) {
                            $(this).text(elem[0].text).css("color", value ? "green" : "red");
                        } else {
                            $(this).text("not-selected").css("color", "red");
                        }

                        if (value) {

                            var elem = sourceData.find(function(item) {
                                return item.value == value;
                            });
                            if (elem) {

                                rule.PropertyName = elem.text;

                                setEditableBlockParameter(rule, $scope);
                            }
                        }
                    }
                });

                if (rule.PropertyName) {

                    var node = $scope.xmlNodes.find(function (node) {
                        return node.Path == rule.PropertyName;
                    });
                    if (node) {
                        blockPropertyName.editable("setValue", node.Id);
                    }// pass
                }
            }

            var setEditableBlockParameter = function (rule, $scope) {

                // block parameter
                var blockParameterName = $("." + ($scope.isDisabledRules ? "suffix" : "") + "ParameterName" + rule.Id);

                blockParameterName.editable("destroy");

                blockParameterName.editable({
                    disabled: $scope.isDisabledRules,
                    prepend: "not-selected",
                    inputclass: 'form-control',
                    source: $scope.validationParameters.map(function (parameter) {
                        return {
                            value: parameter.Id,
                            text: parameter.Name,
                        }
                    }),
                    display: function (value, sourceData) {

                        if (rule.Id < 0) {
                            rule.ParameterId = "";
                            rule.OperatorId = "";
                            rule.Value = "";
                        }// pass

                        var elem = $.grep(sourceData, function (o) {
                            return o.value == value;
                        });

                        if (elem.length) {
                            $(this).text(elem[0].text).css("color", value ? "green" : "red");
                        } else {
                            $(this).text("not-selected").css("color", "red");
                        }

                        if (value) {

                            rule.ParameterId = value;

                            // set operator 
                            setEditableBlockOperator(rule,$scope, value);
                        }

                    }
                });


                if (rule.ParameterId) {
                    blockParameterName.editable("setValue", rule.ParameterId);
                }
            }

            var setEditableBlockOperator = function (rule, $scope, parameterId) {

                // set operators
                var operators = $scope.operatorsToParametersRelations[parameterId];

                var blockOperatorName = $("." + ($scope.isDisabledRules ? "suffix" : "") + "OperatorName" + rule.Id);

                var source = $scope.validationOperators
                    .filter(function (operator) {

                        for (var i = 0; i < operators.length; i++) {
                            if (operators[i] == operator.Id) {
                                return true;
                            }
                        }

                        return false;
                    })
                    .map(function (operator) {
                        return {
                            value: operator.Id,
                            text: operator.Name
                        }
                    });

                blockOperatorName.editable("destroy");
                blockOperatorName.editable({
                    disabled: $scope.isDisabledRules,
                    prepend: "not-selected",
                    inputclass: 'form-control',
                    source: source.length > 0 ? source : [],
                    display: function (value, sourceData) {

                        if (rule.Id < 0) {
                            rule.OperatorId = "";
                        }// pass

                        var elem = $.grep(sourceData, function (o) {
                            return o.value == value;
                        });

                        if (elem.length) {
                            $(this).text(elem[0].text).css("color", value ? "green" : "red");
                        } else {
                            $(this).text("not-selected").css("color", "red");
                        }

                        if (value) {

                            var elem = sourceData.find(function (item) {
                                return item.value == value;
                            })

                            if (elem) {

                                rule.OperatorId = value;
                            }

                            // set value
                            setEditableBlockValue(rule, $scope, parameterId, value);
                        }

                    }
                });

                if (rule.OperatorId) {
                    blockOperatorName.editable("setValue", rule.OperatorId);
                }

            }

            function isEmpty(str) {
                return str.replace(/^\s+|\s+$/g, '').length == 0;
            }

            function trimNbsp(valueForSave) {

                var i = 0;
                var newValue = "";
                while (valueForSave[i] == ' ' && i < valueForSave.length) {
                    newValue += '&nbsp;';
                    i++;
                }
                newValue += valueForSave.slice(i);
                valueForSave = newValue;

                //console.log(valueForSave)
                i = valueForSave.length - 1;
                var newValue = "";
                while (valueForSave[i] == ' ' && i > 0) {
                    newValue = '&nbsp;' + newValue;
                    i--;
                }
                newValue = valueForSave.slice(0, i + 1) + newValue;
                valueForSave = newValue;

                return valueForSave;
            }

            var setEditableBlockValue = function (rule, $scope,  parameterId, operatorId) {

                var blockValueName = $("." + ($scope.isDisabledRules ? "suffix" : "") + "ValueName" + rule.Id);

                if (rule.OperatorId == vm.OPERATORS.Exists.Id ||
                    rule.OperatorId == vm.OPERATORS.NotExist.Id ||
                    rule.OperatorId == vm.OPERATORS.Empty.Id ||
                    rule.OperatorId == vm.OPERATORS.NotEmpty.Id) {

                    // blockValueName.editable("setValue", " ");
                    // //console.log("destroy");
                    blockValueName[0].innerHTML = "";
                    return;
                    // blockValueName.editable("destroy");
                    blockValueName.editable({
                        type: "text",
                        emptytext: "-",
                        display: function (value, sourceData) {
                            return "";
                        }
                    });
                    blockValueName.editable("setValue", " ");
                    blockValueName.editable("destroy");

                    return;
                } 

                var valueData = [];
                var tags = [];
                var editableType = "";
                var min = 0;
                var max = 100;

                if (parameterId == vm.TYPES.DataType.Id) {

                    editableType = "select";
                    valueData = $scope.dataTypes.map(function (dataType) {

                        return {
                            value: dataType.Id,
                            id: dataType.Id,
                            text: dataType.Name
                        }
                    });
                } else if (parameterId == vm.TYPES.Value.Id) {

                    if (operatorId == vm.OPERATORS.ChangesPerValidationMoreThanInPercent.Id
                            || operatorId == vm.OPERATORS.DifferenceBetweeenTherLastEquals.Id
                            || operatorId == vm.OPERATORS.DifferenceBetweeenTherLastLessThan.Id
                            || operatorId == vm.OPERATORS.DifferenceBetweeenTherLastMoreThan.Id) {

                        min = 0;
                        max = 100;
                        editableType = "number";
                    } else {

                        editableType = "datetimeOrDropdowns";
                        valueData = $scope.xmlNodes.map(function (node) {
                            return {
                                value: node.Id,
                                text: node.Path,
                            }
                        });

                        tags = valueData.map(function (data) {
                            return data.text;
                        });
                    }
                } else if (parameterId == vm.TYPES.Length.Id) {

                    if (operatorId == vm.OPERATORS.Between.Id || operatorId == vm.OPERATORS.NotBetween.Id) {

                        editableType = "betweens";
                    } else {
                        editableType = "number";

                    }
                } else if (parameterId == vm.TYPES.NumberOfChanges.Id || parameterId == vm.TYPES.NumberOfDesctinctChanges.Id) {

                    editableType = "numberOfChanges";
                } else {
                    //console.log("pass...");
                }


                if (blockValueName.editable) {
                    ////console.log(blockValueName)

                    if (blockValueName.data("editable")) {

                        if (!blockValueName.data("editable").input.$input)
                            blockValueName.data("editable").input.$input = $({});
                    }

                    blockValueName.editable("destroy");
                }

                $.fn.editabletypes.datetimeOrDropdowns.defaults.dropDownSource = {
                    tags: tags,
                    tokenSeparators: [","]
                };

                blockValueName.editable({
                    disabled: $scope.isDisabledRules,
                    type: editableType,
                    step: 'any',
                    prepend: "not-selected",
                    inputclass: 'form-control',
                    min: min,
                    max: max,
                    source: valueData,
                    display: function (value, sourceData) {

                        if (rule.Id < 0) {
                            rule.Value = "";
                        }// pass
                        
                        if (parameterId == vm.TYPES.DataType.Id || parameterId == vm.TYPES.Value.Id) {
                            if (value) {

                                if (operatorId == vm.OPERATORS.DifferenceBetweeenTherLastEquals.Id ||
                                    operatorId == vm.OPERATORS.DifferenceBetweeenTherLastLessThan.Id ||
                                    operatorId == vm.OPERATORS.DifferenceBetweeenTherLastMoreThan.Id) {

                                    $(this).text(value).css("color", (1 * value >= 0) ? "green" : "red");
                                    vm.setRuleValueById(rule, value, vm.TYPES.Value.Name);
                                } else if (operatorId == vm.OPERATORS.ChangesPerValidationMoreThanInPercent.Id) {

                                    $(this).text("> " + value + "%").css("color", (1 * value >= 0) ? "green" : "red");
                                    vm.setRuleValueById(rule, value, vm.TYPES.Value.Name);
                                } else {
                                    if (parameterId == vm.TYPES.DataType.Id) {

                                        var elem = valueData.find(function (o) {

                                            return o.id == value;
                                        });

                                        if (elem) {
                                            $(this).text(elem.text).css("color", (elem.value >= 0) ? "green" : "red");
                                        } else {
                                            $(this).text("not-selected").css("color", "red");
                                        }

                                        vm.setRuleValueById(rule, elem.text, vm.TYPES.DataType.Name, value);
                                    } else {

                                        if (value) {

                                            rule.IsTime = value.isTime;

                                            
                                            if (!value.isTime && value.text && value.text != "") {

                                                //console.log("-----")
                                                let valueForSave = trimNbsp(value.text);

                                                //console.log(valueForSave)
                                                //                                    //console.log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                                                $(this).text(valueForSave).css("color", "green");
                                                vm.setRuleValueById(rule, valueForSave, vm.TYPES.Value.Name);
                                            } else if (value.isTime) {

                                                $(this).text("NOW - " + value.intervalValue + " " + value.scale).css("color", "green");

                                                vm.setRuleValueById(rule, value.intervalInSeconds, vm.TYPES.Value.Name);
                                            } else {
                                                $(this).text("not-selected").css("color", "red");
                                            }
                                        } else {

                                            $(this).text("not-selected").css("color", "red");
                                        }
                                    }
                                }
                            } else {

                                $(this).text("not-selected").css("color", "red");
                            }
                        } else if (parameterId == vm.TYPES.Length.Id) {

                            if (value && (value >= 0 || (value instanceof Object))) {

                                if (value instanceof Object) {

                                    $(this).text(value.first + " and " + value.second).css("color", "green");
                                    vm.setRuleValueById(rule, value, vm.TYPES.Length.Name, "", operatorId)
                                } else {

                                    $(this).text(value).css("color", "green");
                                    vm.setRuleValueById(rule, value, vm.TYPES.Length.Name)
                                }

                            } else {
                                $(this).text("not-selected").css("color", "red");
                            }
                        } else if (parameterId == vm.TYPES.NumberOfChanges.Id || parameterId == vm.TYPES.NumberOfDesctinctChanges.Id) {

                            if (value && value.numberOfChanges && value.intervalValue && value.intervalInSeconds) {

                                $(this).text(value.numberOfChanges + " for last " + value.intervalValue + " " + value.scale).css("color", "green");

                                if (parameterId == vm.TYPES.NumberOfChanges.Id) {

                                    vm.setRuleValueById(rule, value, vm.TYPES.NumberOfChanges.Name);
                                } else {

                                    vm.setRuleValueById(rule, value, vm.TYPES.NumberOfDesctinctChanges.Name);
                                }
                            } else {

                                $(this).text("not-selected").css("color", "red");
                            }

                        } else {
                            //console.log("pass");

                        }

                        ////console.log("ValueBlock: value changed! :" + value)
                        ////console.log(rule)
                    }
                });

                if (rule.Value || rule.DataTypeId || rule.NumberOfChanges) {

                    if (parameterId == vm.TYPES.DataType.Id) {

                        var index = 0;

                        for (; index < $scope.dataTypes.length; index++) {
                            if ($scope.dataTypes[index].Id == rule.DataTypeId) {
                                blockValueName.editable("setValue", index + 1);
                                break;
                            }// pass
                        }
                    } else if (parameterId == vm.TYPES.Length.Id) {

                        if (operatorId == vm.OPERATORS.Between.Id || operatorId == vm.OPERATORS.NotBetween.Id) {

                            var parts = rule.Value.split("|");

                            blockValueName.editable("setValue", {
                                first: parts[0],
                                second: parts[1]
                            });
                        } else {
                            blockValueName.editable("setValue", rule.Value);
                        }
                    } else if (parameterId == vm.TYPES.NumberOfChanges.Id || parameterId == vm.TYPES.NumberOfDesctinctChanges.Id) {

                        var numberOfChangesValue = vm.buildNumberOfChanges(rule);

                        blockValueName.editable("setValue", numberOfChangesValue);
                    } else {

                        var settingValue = {};

                        if (operatorId == vm.OPERATORS.ChangesPerValidationMoreThanInPercent.Id
                            || operatorId == vm.OPERATORS.DifferenceBetweeenTherLastEquals.Id
                            || operatorId == vm.OPERATORS.DifferenceBetweeenTherLastLessThan.Id
                            || operatorId == vm.OPERATORS.DifferenceBetweeenTherLastMoreThan.Id) {

                            settingValue = rule.Value;
                        } else {

                            if (!rule.IsTime) {

                                settingValue.text = trimNbsp(rule.Value);
                            } else {

                                settingValue = vm.buildNumberOfChanges(rule);
                            }

                            settingValue.isTime = rule.IsTime;
                        }

                        blockValueName.editable("setValue", settingValue);
                    }
                }

            }


            vm.setEditableFor = function ($scope, rule, index) {

                var blockPropertyName = $("." + ($scope.isDisabledRules ? "suffix" : "") + "PropertyName" + rule.Id);
                if (!blockPropertyName.length || blockPropertyName.length === 0) {

                    $timeout(function () {
                        vm.setEditableFor($scope, rule, index);
                    }, 100);

                    return;
                }
                /**
                 * 
                var ruleName = "Rule" + (index == null ? $("a[id^='Rule']").length : index);
                $(".Rule" + rule.Id).text(ruleName);
                rule.RuleName = ruleName;
                
                 */
                //$scope.assignRuleClick();

                setEditableBlockProperty(rule, $scope);
            }

            vm.setEditablesForRules = function ($scope) {
                if ((!$scope.xmlNodes || $scope.xmlNodes.length == 0)) {
                    $timeout(function () {
                        vm.setEditablesForRules($scope);
                    }, 100);
                } else {
                    for (var i = 0; i < $scope.validationRules.length; i++) {
                        var rule = $scope.validationRules[i];
                        vm.setEditableFor($scope, rule, i + 1);
                    }
                }
            };

            return vm;
        });
}());