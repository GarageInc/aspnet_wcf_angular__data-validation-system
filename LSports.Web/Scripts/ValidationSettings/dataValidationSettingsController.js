dataValidationSettingsApp.controller("dataValidationSettingsController", function ($timeout, $scope, crudService, networkService, commonService, dropDownDataSource) {

    $.fn.editable.defaults.mode = 'popup';
    var original = $.fn.editableutils.setCursorPosition;
    $.fn.editableutils.setCursorPosition = function () {
        try {
            original.apply(this, Array.prototype.slice.call(arguments));
        } catch (e) { /* noop */ }
    };

    var vm = this;

    $scope.suffix = "";

    $scope.validationSettings = [];
    $scope.priorities = [];
    $scope.products = [];

    $scope.validationRules = [];
    $scope.validationParameters = [];
    $scope.validationOperators = [];

    $scope.dataTypes = [];
    $scope.xmlNodes = [];

    $scope.expression = "";
    $scope.searchText = "";

    $scope.operatorsToParametersRelations = [];

    $scope.selectedProduct = {};
    $scope.selectedPriority = {};

    $scope.selectedValidationSetting = {}

    $scope.settingId = 1*$("#settingId").val();

    console.log($scope.settingId);

    $scope.collapseTree = function () {
        showHideTree(false);
    }

    $scope.expandTree = function () {
        showHideTree(true);
    }

    var showHideTree = function (isCollaps) {
        //console.log(isCollaps)
        if (isCollaps) {

            tree.jstree("open_all");
        } else {

            tree.jstree("close_all");
        }
    }

    $scope.filter = function () {

        var result = $scope.validationSettings;

        if ($scope.selectedProduct && $scope.selectedProduct.Id && $scope.selectedProduct.Id !== 0) {
            result = result.filter(function (item) {
                return item.Product.Id == $scope.selectedProduct.Id;
            });
        }

        if ($scope.selectedPriority && $scope.selectedPriority.Id && $scope.selectedPriority.Id !== 0) {
            result = result.filter(function (item) {
                return item.Priority.Id == $scope.selectedPriority.Id;
            });
        }

        $scope.setDataProvider(result);
    }

    var table = $("#data_validation_table");

    $scope.refresh = function () {

        //$("#create-edit-validation-settings").modal("show").modal("hide");

        table.DataTable({

            columns: [
                { "data": "Id" },
                { "data": "ProductName" },
                { "data": "Name" },
                { "data": "PriorityName" },
                { "data": "Description" },
                { "data": "edit" },
                { "data": "del" }
            ],
            columnDefs: [
                { "width": "5%", "targets": 0 },
                {
                    "width": "15%", "targets": 1,
                    "type": "string"
                },
                { "width": "15%", "type": "string", "targets": 2 },
                { "width": "15%", "type": "string", "targets": 3 },
                { "width": "40%", "type": "string", "targets": 4 },
                {
                    "width": "10%",
                    "targets": 5,
                    "render": function (data, type, row) {
                        return '<div style="text-align:center"><a style="cursor: pointer;" class="edit"  data-toggle="modal" href="#create-edit-validation-settings">' + data + '</a></div>';
                    }
                },
                {
                    "width": "10%", "targets": 6,
                    "render": function (data, type, row) {
                        return '<div style="text-align:center"><a style="cursor: pointer;" class="delete">' + data + '</a></div>';
                    }
                }
            ]
        });


        $('#data_validation_table tbody').on('click', 'a.delete', function () {

            var row = table.DataTable().row($(this).parents('tr'));

            var idToRemove = table.DataTable().data()[row.index()].Id;

            bootbox.confirm("Are you sure you want to delete this validation setting?", function (result) {
                if (result) {
                    crudService.remove("ValidationSettings", idToRemove).then(function () {
                        table.DataTable().row(row).remove().draw();
                        toastr["success"]("Validation setting was successfully deleted!");
                    });
                }
            });
        });

        $('#data_validation_table tbody').on('click', 'a.edit', function () {
            var row = table.DataTable().row($(this).parents('tr'));

            var idSelected = table.DataTable().data()[row.index()].Id;

            $scope.setSelected(idSelected);
        });

        loadValidationSettings();

        dropDownDataSource.sendRequest(dropDownDataSource.requestBuilder().setPath("/Priorities").build()).then(function (data) {

            $scope.priorities = data;
        });

        dropDownDataSource.sendRequest(dropDownDataSource.requestBuilder().setPath("/Products").build()).then(function (data) {

            $scope.products = data;
        });

        crudService.get("ValidationSettings", "GetValidationOperators").then(function (data) {

            $scope.validationOperators = data;
        });

        crudService.get("ValidationSettings", "GetValidationParameters").then(function (data) {

            $scope.validationParameters = data;
        });

        crudService.get("Product", "GetOperatorsToParametersRelations").then(function (data) {

            $scope.operatorsToParametersRelations = data;
        });

        crudService.get("ValidationSettings", "GetValidationDataTypes").then(function (data) {

            $scope.dataTypes = data;
        });

    }

    var loadValidationSettings = function () {

        crudService.list("ValidationSettings").then(function (data) {

            $scope.validationSettings = [];

            $scope.validationSettings = data.map(function(item) {

                return {
                    Setting: {
                        Id: item.Id,
                        Name: item.Name,

                        ProductId: item.Product.Id,
                        ProductName: item.Product.Name,
                        Product: item.Product,

                        Priority: item.Priority,
                        PriorityId: item.Priority.Id,
                        PriorityName: item.Priority.Name,

                        Description: item.Description,
                        Expression: item.Expression,

                        edit: "edit",
                        del: "delete"
                    }
                };
            });

            $scope.setDataProvider($scope.validationSettings.map(function(item) {
                return item.Setting;
            }));

            if ($scope.settingId && $scope.settingId > 0) {

                $scope.setSelected($scope.settingId);
                $("#create-edit-validation-settings").modal("show");
            }
        });
    }

    $scope.setDataProvider = function (_dataProvider) {

        table.DataTable().clear().draw();
        table.DataTable().rows.add(_dataProvider).draw();
    }
    $scope.setSelected = function (idSelected) {
        $scope.searchText = ""; 
        $scope.rulesIndexId = -1;

        rulesIdsForDeleting = [];
        $scope.validationRules = [];

        var selectedValidationSetting = $scope.validationSettings.find(function (item) {
            return item.Setting.Id == idSelected;
        });

        if (selectedValidationSetting) {

            $scope.selectedValidationSetting = selectedValidationSetting;

            $scope.expression = selectedValidationSetting.Setting.Expression;

            crudService.get("ValidationSettings", "GetValidationRules", { validationSetttingId: $scope.selectedValidationSetting.Setting.Id }).then(function (data) {

                $scope.validationRules = data;

                commonService.setEditablesForRules($scope);

                $scope.loadXMLData();
            });
        } else {
            console.log($scope.selectedProduct);
            $scope.selectedValidationSetting = {
                Setting: {
                    Id: 0,
                    Name: "",
                    Description: "",
                    Expression: ""
                }
            }
        }
    }

    $scope.wasSelectedProduct = function () {

        var product = $scope.products.find(function(product) {
            return product.Id == $scope.selectedValidationSetting.Setting.ProductId;
        });

        if (product) {
            $scope.selectedValidationSetting.Setting.ProductName = product.Name;
        }

        $scope.loadXMLData();
        $("#select-product").modal("hide");
        $("#create-edit-validation-settings").modal("show");
    }

    var tree = $("#jsTree");
    $scope.lastSelectedNode = "";
    $scope.rulesIndexId = -1;

    $scope.searchInTree = function(event) {
        tree.jstree(true).search(event.target.value);
    }

    $scope.loadXMLData = function () {

        crudService.get("Product", "GetXMLData", { productId: $scope.selectedValidationSetting.Setting.ProductId })
            .then(function (data) {
//                console.log(data)
                tree.jstree('destroy');
                data.plugins = ["search"];
                tree.jstree(data);

                tree.on('search.jstree',
                    function(e, data) {
                        if (data.nodes.length) {
                            tree.jstree(true).get_node(data.nodes[0].id, true).children('.jstree-anchor')[0].scrollIntoView();
                        }
                    });

                tree.on('loaded.jstree',
                    function () {
                        showHideTree(true);

                        $scope.xmlNodes = [];

                        $scope.searchText = "";

                        var selfTree = tree.jstree();

                        var index = 1;
                        $('.jstree-leaf')
                            .each(function () {

                                var node = selfTree.get_node(this);

                                if (node.children.length !== 0)
                                    return;

                                var nodePath = selfTree.get_path(node).join(".");

                                $scope.xmlNodes.push({
                                    Id: index++,
                                    Path: nodePath//.slice(4)
                                });

                            });
                    });

                tree.bind("dblclick.jstree",
                    function (event) {
                        var tree = $(this).jstree();

                        var node = tree.get_node(event.target);

                        if (node.children.length !== 0)
                            return;

                        var nodePath = tree.get_path(node).join(".");

                        var newRule = {
                            Id: $scope.rulesIndexId,
                            RuleName: "Rule" + $scope.rulesIndexId,
                            PropertyName: nodePath,//.slice(4),
                            ParameterId: 0,
                            OperatorId: 0,
                            DataTypeId: 0,
                            TimeFrame: "",
                            NumberOfChanges: 0,
                            Value: "",
                            IsActive: 1,
                            IsTime: false,
                            IsForAllNodes: false
                        }

                        $scope.lastSelectedNode = nodePath;//.slice(4);

                        $scope.validationRules.push(newRule);

                        $scope.$apply();

                        $scope.rulesIndexId--;

                        commonService.setEditableFor($scope, newRule);
                    });
            });
    }

    $scope.saveChanges = function () {

        if (isValidatedForm()) {

            console.log($scope.validationRules);

            var validationSetting = {

                Id: $scope.selectedValidationSetting.Setting.Id,
                Name: $scope.selectedValidationSetting.Setting.Name,
                Description: $scope.selectedValidationSetting.Setting.Description,

                PriorityId: $scope.selectedValidationSetting.Setting.PriorityId,
                ProductId: $scope.selectedValidationSetting.Setting.ProductId,

                ValidationRules: $scope.validationRules,
                Expression: $scope.expression
            }

            if (validationSetting.Id) {
                crudService.update("ValidationSettings", validationSetting).then(function () {
                    loadValidationSettings();
                });
            } else {
                crudService.create("ValidationSettings", validationSetting).then(function () {
                    loadValidationSettings();
                });;
            }

            for (var i = 0; i < rulesIdsForDeleting.length; i++) {
                crudService.remove("ValidationRules", rulesIdsForDeleting[i]);
            }

            rulesIdsForDeleting = [];
            $("#create-edit-validation-settings").modal("hide");

            toastr["success"]("All changes was succesfully saved");
        } else {
            toastr["error"]("Can't save validation setting! </br> Check all required parameter at validation rules!");
        }
    }



    var isValidatedForm = function () {
        if ($scope.selectedValidationSetting.Setting.Description == "") {
            return false;
        }

        if ($scope.selectedValidationSetting.Setting.Name == "") {
            return false;
        }

        if ($scope.selectedValidationSetting.Setting.PriorityId == "") {
            return false;
        }

        if ($scope.selectedValidationSetting.Setting.ProductId == "") {
            return false;
        }

        for (var i = 0; i < $scope.validationRules.length; i++) {
            var rule = $scope.validationRules[i];

            if (!rule.PropertyName || rule.PropertyName == "") {
                return false;
            }

            if (!rule.ParameterId || rule.ParameterId == 0) {
                return false;
            }

            if (!rule.OperatorId || rule.OperatorId == 0) {
                return false;
            }

            var passByOperator = rule.OperatorId == commonService.OPERATORS.Exists.Id ||
                rule.OperatorId == commonService.OPERATORS.NotExist.Id ||
                rule.OperatorId == commonService.OPERATORS.Empty.Id ||
                rule.OperatorId == commonService.OPERATORS.NotEmpty.Id;

            if (passByOperator == false && (!rule.Value || rule.Value === "")) {
                return false;
            }
        }

        return true;
    }

    var rulesIdsForDeleting = [];

    $scope.addRuleToTextArea = function (event, textAreaId, name) {

        var ruleName = " " + name + " ";

        setToTextArea(textAreaId, ruleName, ruleName);
    }

    $scope.addToExpression = function (text, textAreaId) {
        text = " " + text + " ";

        setToTextArea(textAreaId, "", text);
    }

    function setToTextArea(textAreaId, defaultValue, setValue) {

        var textArea = $("#" + textAreaId);
        var position = textArea.prop('selectionStart');

        $scope.expression = $scope.expression == null ? defaultValue :
                                                        [$scope.expression.slice(0, position), setValue, $scope.expression.slice(position)].join('');
    }

    $scope.deleteRule = function (ruleId) {

        bootbox.confirm("Are you sure you want to delete this validation rule?", function (result) {
            if (result) {
                for (var i = 0; i < $scope.validationRules.length; i++) {
                    if ($scope.validationRules[i].Id == ruleId) {

                        $scope.validationRules.splice($scope.validationRules.indexOf($scope.validationRules[i]), 1);

                        if (ruleId > 0) {

                            rulesIdsForDeleting.push(ruleId);
                        }

                        for (var j = 0; j < $scope.validationRules.length; j++) {

                            var rule = $scope.validationRules[j];
                            var ruleName = "Rule" + (j + 1);
                            $("#Rule" + rule.Id).text(ruleName);
                            rule.RuleName = ruleName;
                        }

                        return;
                    }
                }
            }
        });
    }


    return vm;
});