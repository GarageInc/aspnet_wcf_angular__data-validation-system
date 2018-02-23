
validationResultsApp.controller("validationResultsController", function (
    $timeout, $interval, $scope,
    crudService, commonService, $rootScope,
    dropDownDataSource, smartDropdowns) {
	
	var dateFormat = 'HH:mm:ss DD-MM-YYYY';


	$.fn.dataTable.moment(dateFormat);

    Array.prototype.unique = function () {
        var o = {}, i, l = this.length, r = [];
        for (i = 0; i < l; i += 1) o[this[i]] = this[i];
        for (i in o) r.push(o[i]);
        return r;
    };
    $('#validation_settings_table, #common_validation_results_table, #event_validation_results_table').mousedown(function () {
        
    });

    var vm = this;

    var settingsTable = $("#validation_settings_table");
    var commonValidationResultsTable = $("#common_validation_results_table");
    var eventValidationResultsTable = $("#event_validation_results_table");

    var rulesIdsForDeleting = [];

    $scope.isForMarket = true;
    $scope.isForProvider = true;

    $scope.offset = 0;
    $scope.counter = 10000;
    $scope.multips = [];

    $scope.priorities = [];
    $scope.products = [];
    $scope.operatorsToParametersRelations = [];

    $scope.validationResults = [];
    $scope.validationSettingsModels = [];
    $scope.validationRules = [];

    $scope.isLoading = true;

    $scope.productId = $("#productId").val();

    $scope.drowDownsFilterObject = {}

    $scope.isForMarketProvider = true;
    $scope.isHistoricalAttributes = false;
    $scope.isWithHeader = false;
    $scope.isLoadHistoricalResults = false;
    $scope.XmlMessage = "";

    $scope.selectedProduct = {}
    $scope.selectedPriority = {};
    $scope.selectedValidationSetting = undefined;

    $scope.filterObject = {
        Sport: { Id: null, name: null },
        Country: { Id: null, name: null },
        League: { Id: null, name: null },
        Event: { Id: null, name: null },
        Market: { Id: null, name: null },
        Provider: { Id: null, name: null },
        EventStatus: { Id: null, name: null },
    }

    $scope.enablings = [
        true,
        false
    ];

    $scope.$on("loading", function (event, data) {

        $scope.isLoading = true;
    });
    $scope.$on("loaded", function (event, data) {

        $scope.isLoading = false;
    });

    $scope.isShowingRules = false;
    $scope.isDisabledRules = true;

    $scope.$on("showRulesTable", function (event, data) {

        $scope.isShowingRules = !$scope.isShowingRules;

        if ($scope.isShowingRules) {

            $timeout(function () {
                $scope.isDisabledRules = true;
                commonService.setEditablesForRules($scope);
            }, 1000);
        }
    });

    $scope.$on("timeframechanged", function (event, data) {
        if (data) {
            $("#dateFrom").editable("setValue", (data.startDate));
            $("#dateTo").editable("setValue", (data.endDate));
        }

        if ($scope.isLoading == false) {
            loadValidationSettings();
        }
    });


    $scope.checkboxIntervalChanged = function () {

        if ($scope.updateByInterval) {

            loadValidationSettings();
        }
    }

    $scope.getDateFromMilliseconds = function (data) {

        if (!data)
            return "<can not be defined>";

        var splicedTime = data.slice(6, data.length - 2);

        if (splicedTime > 0) {

            var date = new Date();
            date.setTime(splicedTime * 1);

            return moment(date).format(dateFormat);
        } else {

            return "";
        }
    }

    var DEFAULT_PAGE_LENGTH = 10;

    var initSettingsDataTable = function () {

        var settingsDataTable = settingsTable.DataTable({
            pageLength: +(localStorage.getItem("validation_settings_table_page_length") || DEFAULT_PAGE_LENGTH),
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            columns: [
                { "data": "Id" },
                { "data": "Name" },
                { "data": "ProductName"},
                { "data": "PriorityName" },
                { "data": "OpenIssues" },
                { "data": "TotalIssues" },
                { "data": "LastTimeShown" }
            ],
            columnDefs: [
                { "targets": 0 },
                { "targets": 1 },
                { "targets": 2 },
                {
                    "type": "string", "targets": 3,
                    "render": function (data) {

                        if (data == "Low") {

                            return '<div class="relative"><div class="absolute inline low_priority_arrow"></div>' + '<div class="absolute inline l30p">' + data + '</div></div>';
                        } else if (data == "Medium") {

                            return '<div class="relative"><div class="absolute inline medium_priority_arrows"></div>' + '<div class="absolute inline l30p">' + data + '</div></div>';
                        }else if (data == "High") {

                            return '<div class="relative"><div class="absolute inline high_priority_arrows"></div>' + '<div class="absolute inline l30p">' + data + '</div></div>';
                        } else {

                            return "Can't be defined";
                        }
                    }
                },
                {
                    "type": "html-num-fmt", "targets": 4,
                    "render": function (data) {

                        if (data * 1 == 0) {

                            return '<div style="position:relative;" ><div class="inline">' + data + '</div>' +'</div>';
                        } else if (data*1 >=1 && data * 1 <= 5) {

                            return '<div style="position:relative;" ><div class="inline">' + data + '</div>' + '<div class="circle inline custom_badge" style="background-color: #c0d9b5;"></div></div>';
                        }
                        else if (data * 1 >= 6 && data * 1 <= 50) {

                            return '<div style="position:relative;" ><div class="inline">' + data + '</div>' + '<div class="circle inline custom_badge" style="background-color: #ffecb3;"></div></div>';
                        }
                        else if (data * 1 >= 51 && data * 1 <= 200) {

                            return '<div style="position:relative;" ><div class="inline">' + data + '</div>' + '<div class="circle inline custom_badge" style="background-color: #f3c89c;"></div></div>';
                        } else {

                            return '<div style="position:relative;" ><div class="inline">' + data + '</div>' + '<div class="circle inline custom_badge" style="background-color: #e7aaaa;"></div></div>';
                        }
                    }
                },
                { "type": "number", "targets": 5 },
                {
                	//"type": "datetime-moment",
                	"targets": 6,
                    "render": function (data, type, row) {
                        return $scope.getDateFromMilliseconds(data);
                    }
                }
            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                nRow.className = "pointer";
            }
        });

        $('#validation_settings_table').on('length.dt', function (e, settings, len) {
            localStorage.setItem("validation_settings_table_page_length", len);
        });

        $("#validation_settings_table tbody")
            .on("mouseenter", "tr", function () {

                if (!settingsDataTable.cell(this).index()) {
                    return;
                }
                var rowIdx = settingsDataTable.cell(this).index().row;

                $(settingsDataTable.cells().nodes()).removeClass("bordered");
                $(settingsDataTable.column(rowIdx).nodes()).addClass("bordered");
            });

        var load = function(el) {

            var row = settingsTable.DataTable().row($(el).parents("tr"));
            var tableData = settingsTable.DataTable().data();

            var settingId = tableData[row.index()].Id;

            $scope.offset = 0;

            setSelectedValidationSetting(settingId);

            loadValidationRules(true);
            
            $scope.$apply();

            $scope.isShowingRules = false;
            $scope.isHistoricalAttributes = false;
            $scope.isWithHeader = false;

            $scope.multips = [];
            for (var i = 0; i <= $scope.selectedValidationSetting.TotalIssues / $scope.counter; i++) {
                $scope.multips.push(i);
            }
            
            loadValidationResultsBySetting();
        }

        var body = $("#validation_settings_table tbody");
        
        body.on("click", "td", function (event) {
            
            load(this);
        });

    }

    var loadValidationRules = function(value) {
        return crudService.get("ValidationSettings", "GetValidationRules", { validationSetttingId: $scope.selectedValidationSetting.Setting.Id })
                .then(function (data) {
                    $scope.validationRules = data;
                })
                .then(function () {

                    $scope.isDisabledRules = value;
                    commonService.setEditablesForRules($scope);
                    $scope.loadXMLData();
                });
    }

    $scope.deleteSetting = function () {

        var id = $scope.selectedValidationSetting.Setting.Id;
        var name = $scope.selectedValidationSetting.Setting.Name;

        if (id <= 0) {

            $('.modal').modal('hide');
        } else {

            bootbox.confirm('Are you sure you want to delete this validation setting "' + name + '"?', function (result) {
                if (result) {
                    $rootScope.$broadcast("loading");
                    crudService.remove("ValidationSettings", id).then(function () {

                        toastr["success"]("Validation setting was successfully deleted!");
                        loadValidationSettings();
                        $('.modal').modal('hide');
                    });
                }
            });
        }
    }

    $scope.canDelete = function () {
        return $scope.selectedValidationSetting && $scope.selectedValidationSetting.Setting.Id > 0;
    }

    var initCommonValidationResultsDataTable = function () {

        commonValidationResultsTable.DataTable({
            pageLength: +(localStorage.getItem("common_validation_results_table_page_length") || DEFAULT_PAGE_LENGTH),
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            columns: [
                { "data": "EventId" },
                { "data": "EventName" },
                { "data": "SportName" },
                { "data": "CountryName" },
                { "data": "LeagueName" },
                { "data": "StatusName" },
                { "data": "MarketsCount" },
                { "data": "ProvidersCount" },
                { "data": "ValidationResultsCount" },
                { "data": "LastTimeShown" }
            ],
            columnDefs: [
                { "targets": 0 },
                { "type": "string", "targets": 1 },
                { "type": "string", "targets": 2 },
                { "type": "string", "targets": 3 },
                { "type": "string", "targets": 4 },
                { "type": "string", "targets": 5 },
                
                {
                    "type": "html-num-fmt", "targets": 6,
                    "render": function (data, type, row) {
                        return '<div style="text-align:center">' + data + "</div>";
                    }
                },
                {
                    "type": "html-num-fmt", "targets": 7,
                    "render": function (data, type, row) {
                        return '<div style="text-align:center">' + data + "</div>";
                    }
                },
                {
                    "type": "html-num-fmt", "targets": 8,
                    "render": function (data, type, row) {
                        return '<div style="text-align:center">' + data + "</div>";
                    }
                },
                {
                	//"type": "datetime-moment",
                	"targets": 9,
                    "render": function (data, type, row) {
                        return $scope.getDateFromMilliseconds(data);
                    }
                },
                {
                    "type": "string", "targets": 10,
                    "render": function (data, type, row) {
                        if (row.IsActive || row.IsActive === 'true') {

                            return '<div style="text-align:center"><a style="cursor: pointer;" class="disableValidationResults">Disable</a></div>';
                        } else {

                            return '<div style="text-align:center">Historical</div>';
                        }
                    }
                }
            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                nRow.className = "pointer";
            }
        });

        $('#common_validation_results_table').on('length.dt', function (e, settings, len) {
            localStorage.setItem("common_validation_results_table_page_length", len);
        });

        $('#common_validation_results_table tbody').on('click', 'a.disableValidationResults', function (e) {

            e.preventDefault();
            e.stopPropagation();

            var row = commonValidationResultsTable.DataTable().row($(this).parents('tr'));
            var eventId = commonValidationResultsTable.DataTable().data()[row.index()].EventId;

            disableSelectedValidationResults($scope.selectedProduct.Id, $scope.selectedValidationSetting.Setting.Id, eventId)
                .then(loadValidationSettings);
        });

        var body = $("#common_validation_results_table tbody");

        body.on("click", "td", function (event) {

            var row = commonValidationResultsTable.DataTable().row($(this).parents("tr"));
            var tableData = commonValidationResultsTable.DataTable().data();

            var eventId = tableData[row.index()].EventId;

            showResultsByEventId(eventId);
        });
    }

    $scope.selectedEventId = undefined;

    var showResultsByEventId = function(eventId) {
        $scope.selectedEventId = eventId;

        var valResultsForEvent = $scope.validationResults.filter(function(item) {
            return item.EventId === eventId;
        });

        var _dataProvider = valResultsForEvent.map(function (item) {
            return {
                Id: item.Id,
                EventId: item.EventId,
                EventName: item.EventName,
                LeagueName: item.LeagueName,
                Markets: item.Market && item.Market != "" ? item.Market.split(',') : [],
                Providers: item.Provider && item.Provider != "" ? item.Provider.split(',') : [],
                StatusName: item.Status,
                SportName: item.SportName,
                CountryName: item.LocationName,
                Counter: item.Counter,
                LastTimeShown: item.UpdatedOn,
                IsActive: item.IsActive
            }
        });

        setDataProvider(eventValidationResultsTable, _dataProvider);

        $("#events_validation_results_modal").modal("show");
    }

    

    var initEventValidationResultsDataTable = function () {

        eventValidationResultsTable.DataTable({
            pageLength: +(localStorage.getItem("event_validation_results_table_page_length") || DEFAULT_PAGE_LENGTH),
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            columns: [
                { "data": "EventId" },
                { "data": "SportName" },
                { "data": "CountryName" },
                { "data": "LeagueName" },
                { "data": "EventName" },

                { "data": "Markets" },
                { "data": "Providers" },
                { "data": "StatusName" },
                { "data": "Counter" },
                { "data": "LastTimeShown" }
            ],
            columnDefs: [
                { "targets": 0 },
                { "type": "string", "targets": 1 },
                { "type": "string", "targets": 2 },
                { "type": "string", "targets": 3 },
                {
                    "type": "string", "targets": 4,
                    "render": function (data, type, row) {

                        if (!data) {
                            return "<not found>";
                        } else {
                            return data;
                        }
                    }
                },
                {
                    "type": "string", "targets": 5,
                    "render": function (data, type, row) {
                        if (!data || data.length === 0) {
                            return "<not found>";
                        } else {
                            return data.join('</br>');
                        }
                    }
                },
                {
                    "type": "string", "targets": 6,
                    "render": function (data, type, row) {

                        if (!data || data.length === 0) {
                            return "<not found>";
                        } else {
                            return data.join('</br>');
                        }
                    }
                },
                { "type": "string", "targets": 7 },
                {
                    "type": "html-num-fmt", "targets": 8,
                    "render": function (data, type, row) {

                        return '<div style="text-align:center">' + data + "</div>";
                    }
                },
                {
                	//"type": "datetime-moment",
                	"targets": 9,
                    "render": function (data, type, row) {
                        return $scope.getDateFromMilliseconds(data);
                    }
                }
            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                nRow.className = "pointer";
            }
        });

        $('#event_validation_results_table').on('length.dt', function (e, settings, len) {
            localStorage.setItem("event_validation_results_table_page_length", len);
        });

        var body = $("#event_validation_results_table tbody");

        body.on("click", "td", function (event) {

            var row = eventValidationResultsTable.DataTable().row($(this).parents("tr"));
            var tableData = eventValidationResultsTable.DataTable().data();

            var resultId = tableData[row.index()].Id;

            setSelectedValidationResult(resultId);

            loadXml($scope.selectedValidationResult);
        });
    }

    var disableSelectedValidationResults = function (productId, settingId, eventId) {

        $rootScope.$broadcast("loading");

        return crudService.get("ValidationResults", "DisableForSettingEvent",
        {
            productId: productId,
            settingId: settingId,
            eventId: eventId
        }).then(function (data) {

            $scope.commonValidationResults = $scope.commonValidationResults.filter(function(item) {
                return +item.EventId !== +eventId;
            });

            setDataProvider(commonValidationResultsTable, $scope.commonValidationResults);
        });
    }

    $scope.loadBySavedXml = function () {
        processXml($scope.selectedValidationResult, $scope.XmlMessage);
    }

    $scope.changeIsHistoricalAttributes = function () {
        loadXml($scope.selectedValidationResult);
    }


    var dropOther = function (loadingXml, allNodesReg, mainReg) {

        var allNodesMatch = loadingXml.match(allNodesReg);

        if (allNodesMatch) {

            for (var i = 0; i < allNodesMatch.length; i++) {
                if (!allNodesMatch[i].match(mainReg)) {
                    // console.log("replaced!")
                    loadingXml = loadingXml.replace(allNodesMatch[i], "");
                    loadingXml = loadingXml.replace(new RegExp("^(?:[\t ]*(?:\r?\n|\r))+", "gm"), "");// /^\s*[\r\n]/gm
                }
            }
        }

        return loadingXml;
    }

    function groupByMultiply(array, f) {
        var groups = {};
        array.forEach(function (o) {
            var group = JSON.stringify(f(o));
            groups[group] = groups[group] || [];
            groups[group].push(o);
        });
        return Object.keys(groups)
            .map(function(group) {
                return groups[group];
            });
    }

    $scope.canReduceMarket = function() {
        if ($scope.selectedValidationResult) {

            return $scope.selectedValidationResult.Market &&
                $scope.selectedValidationResult.Market != "" &&
                $scope.selectedValidationResult.Market.indexOf(',') === -1;
        } else {
            return false;
        }
    }

    $scope.canReduceProvider = function () {
        if ($scope.selectedValidationResult) {

            return $scope.selectedValidationResult.Provider && $scope.selectedValidationResult.Provider != "" && $scope.selectedValidationResult.Provider.indexOf(',') === -1;
        } else {
            return false;
        }
    }

    var processXml = function (validationResult, xml) {

        $("#modal_showing_xml").modal("show");

//        console.log("before: " + xml.length);

        var loadingXml = vkbeautify.xml(xml);

        loadingXml = replaceSymbols(loadingXml);

        var pointsToHighline = validationResult.PointsToHighline.split("|");
        pointsToHighline = pointsToHighline.filter(onlyUnique);
        pointsToHighline = pointsToHighline.map(function (item) {

            var parts = item.split("@");

            return{
                parentNodeName: parts[0],
                parentNodeIdentifier: parts[1],
                line: parts[2]
            }
        });

        var groupedPoints = groupByMultiply(pointsToHighline, function (item) {
            return item.parentNodeName + item.parentNodeIdentifier;
        });

        console.log(groupedPoints);

        var allOutcomesReg = new RegExp("&ltOutcome [\\s\\S]*?&gt" + "[\\s\\S]*?" + "&lt/Outcome&gt?", "g");
        var mainOutcomeReg = new RegExp('&ltOutcome id="[0-9]*" name="' + validationResult.Market + '"&gt[\\s\\S]*?' + "&lt/Outcome&gt?", "g");

        var allBookmakersReg = new RegExp("&ltBookmaker [\\s\\S]*?&gt" + "[\\s\\S]*?" + "&lt/Bookmaker&gt?", "g");
        var mainBookmakerReg = new RegExp('&ltBookmaker id="[0-9]*" name="' + validationResult.Provider + '" [\\s\\S]*?&lt/Bookmaker&gt?', "g");

        var canReduce = $scope.canReduceMarket() && $scope.canReduceProvider();

        if ($scope.canReduceMarket() && $scope.isForMarket) {
                
            loadingXml = dropOther(loadingXml, allOutcomesReg, mainOutcomeReg);
            
            if ($scope.canReduceProvider() && $scope.isForProvider) {
                    
                loadingXml = dropOther(loadingXml, allBookmakersReg, mainBookmakerReg);
            }
        }


        if ($scope.isWithHeader == false) {
            loadingXml = loadingXml.replace(new RegExp("&ltHeader&gt" + "[\\s\\S]*?" + "&lt/Header&gt", "g"), "");
            loadingXml = loadingXml.replace(new RegExp("^(?:[\t ]*(?:\r?\n|\r))+", "gm"), "");// /^\s*[\r\n]/gm
        }

        var highlinedObjects = {};

        for (var groupIndex = 0; groupIndex < groupedPoints.length; groupIndex++) {
            var points = groupedPoints[groupIndex];

            for (var k = 0; k < points.length; k++) {
                var isLastForNode = points.length - 1 === k;

                var point = points[k];

                var parentNodeName = point.parentNodeName;
                var line = point.line;
                var parentNodeIdentifier = point.parentNodeIdentifier;

                if (parentNodeName === '' && line === '' && parentNodeIdentifier === ''
                    || !line && !parentNodeIdentifier && parentNodeName === '') {
                    continue;
                }

                var id = parentNodeIdentifier + line;

                if (highlinedObjects[id]) {
                    console.log("pass", id);
                    continue;
                }

                var lineWithBlockedSymbols = line.replace(new RegExp('\\(', 'g'), '\\(');
                lineWithBlockedSymbols = lineWithBlockedSymbols.replace(new RegExp('\\)', 'g'), '\\)');

                highlinedObjects[id] = id;

                var blocks = undefined;
                var isLineEqualToIdentifier = (line === parentNodeIdentifier);

                if (parentNodeName === "Odds" || parentNodeName === "Odd" && canReduce) {// for odds

                    var outcomes = loadingXml.match(mainOutcomeReg);

                    if (outcomes && line != "") {

                        var outcome = outcomes[0];

                        var bookmakers = outcome.match(mainBookmakerReg);

                        if (bookmakers) {
                            var bookmaker = bookmakers[0];

                            if (isLineEqualToIdentifier) {
                                blocks = bookmaker.match(new RegExp("&lt" + parentNodeName + " [\\s\\S]{0,200}?" + lineWithBlockedSymbols + "?[\\s\\S]*?&gt[\\s\\S]*?&gt", "g"));
                            } else {
                                blocks = bookmaker.match(new RegExp("&lt" + parentNodeName + " [\\s\\S]{0,200}?" + parentNodeIdentifier + "[\\s\\S]{0,350}?" + lineWithBlockedSymbols + "?[\\s\\S]*?/&gt", "g"));
                            }
                        }
                    }
                } else {
                    // for values in nodes, but not for attributes
                    blocks = loadingXml.match(new RegExp("&lt" + parentNodeName + " [\\s\\S]{0,200}?" + parentNodeIdentifier + "[\\s\\S]{0,200}?&gt" + lineWithBlockedSymbols + "[\\s\\S]*?&lt/" + parentNodeName + "&gt", "g"));

                    blocks = blocks || loadingXml.match(new RegExp("&lt" + parentNodeName + "[\\s\\S]{0,200}?" + parentNodeIdentifier + "[\\s\\S]{0,200}?&gt" + lineWithBlockedSymbols + "[\\s\\S]*?&lt/" + parentNodeName + "&gt", "g"));
                }

                // for attributes
                if ((!blocks || +blocks.length === 0)) {
                    if (line !== "") {
                        if (isLineEqualToIdentifier) {
                            blocks = loadingXml.match(new RegExp("&lt" + parentNodeName + " [\\s\\S]{0,200}?" + lineWithBlockedSymbols + "?[\\s\\S]*?&gt", "g"));
                            
                        } else {
                            blocks = loadingXml.match(new RegExp("&lt" + parentNodeName + " [\\s\\S]{0,200}?" + parentNodeIdentifier + "[\\s\\S]{0,350}?" + lineWithBlockedSymbols + "?[\\s\\S]*?&gt", "g"));
                            //console.log(new RegExp("&lt" + parentNodeName + " [\\s\\S]{0,200}?" + parentNodeIdentifier + "[\\s\\S]{0,350}?" + lineWithBlockedSymbols + "?[\\s\\S]*?&gt?"))
                        }
                    } else {
                        line = "&lt" + parentNodeName;
                        blocks = loadingXml.match(new RegExp("&lt" + parentNodeName + " [\\s\\S]{0,200}?&gt", "g"));
                    }
                }

                if (lineWithBlockedSymbols === "") {
                    lineWithBlockedSymbols = line;
                }

                if (blocks) {
                    for (var i = 0; i < blocks.length; i++) {
                        var highlinedBlock = replace(blocks[i], lineWithBlockedSymbols, line, "red");

                        if (isLineEqualToIdentifier === false && parentNodeIdentifier !== "") {
                            // console.log(line);
                            if (isLastForNode) {
                                //console.log(points);
                                highlinedBlock = replace(highlinedBlock, parentNodeIdentifier, parentNodeIdentifier, "blue");
                            }
                        }

                        loadingXml = loadingXml.replace(blocks[i], highlinedBlock);
                    }
                }
            }
        }
        

        var forXml = document.getElementById("xmlValue");
        forXml.innerHTML = loadingXml;
    }

    var loadXml = function (validationResult) {
        if (!validationResult) {
            toastr["error"]("Don't have xml to show!");
        }

        $rootScope.$broadcast("loading");

        crudService.get("ValidationResults", "GetXmlForValidationResult",
        {
            eventId: validationResult.EventId,
            isHistoricalAttributes: $scope.isHistoricalAttributes,
            productId: validationResult.ProductId
        }).then(function (xml) {

            //console.log(xml);

            $scope.XmlMessage = xml;

            processXml($scope.selectedValidationResult, $scope.XmlMessage);

            $rootScope.$broadcast("loaded");
        });
    }


    function replace(loadingXml, blockedLine, originalLine, color) {

        loadingXml = highline(loadingXml, replaceSymbols(blockedLine), replaceSymbols(originalLine), color);

        return loadingXml;
    }

    function highline(source, blockedLine, originalLine, color) {

        var reg = new RegExp(blockedLine, "g");

        source = source.replace(reg, '<em style="color: ' + color + ';">' + originalLine + "</em>")

        return source;
    }


    function replaceSymbols(xml) {
        var first = xml.replace(new RegExp("<", "g"), "&lt");
        var second = first.replace(new RegExp(">", "g"), "&gt");

        return second;
    }
    
    function onlyUnique(value, index, self) {
        return self.indexOf(value) === index;
    }


    $scope.sports = [];
    $scope.countries = [];
    $scope.leaques = [];
    $scope.events = [];
    $scope.markets = [];
    $scope.providers = [];
    $scope.statuses = [];

    $scope.loadNextDropdownsByType = function (filterType) {
        return smartDropdowns.callByType($scope, $scope.selectedProduct ? $scope.selectedProduct.Id : null, filterType)
    }

    var setDataProvider = function (table, _dataProvider) {

        table.DataTable().clear().draw();
        table.DataTable().rows.add(_dataProvider).draw();
    }

    $scope.filterSettings = function () {
        
        var models = $scope.validationSettingsModels;

        if ($scope.selectedPriority && $scope.selectedPriority.Id && $scope.selectedPriority.Id !== 0) {
            models = models.filter(function (item) {
                return item.Setting.Priority.Id == $scope.selectedPriority.Id;
            });
        }

        if ($scope.selectedProduct && $scope.selectedProduct.Id && $scope.selectedProduct.Id !== 0) {
            models = models.filter(function (item) {
                return item.Setting.ProductId == $scope.selectedProduct.Id;
            });
        }
        
        setDataProvider(settingsTable, getSettingsTableDataProvider(models));
    }

    $scope.getDescriptionOfSetting = function () {

        if (!$scope.selectedValidationSetting) {
            return "-";
        }

        return $scope.selectedValidationSetting.Setting.Description
            ? $scope.selectedValidationSetting.Setting.Description
            : 'no description';
    }

    function getSettingsTableDataProvider(settings) {

        return settings.map(function (item) {
            return {
                Id: item.Setting.Id,
                Name: item.Setting.Name,
                ProductName: item.Setting.Product.Name,
                PriorityName: item.Setting.Priority.Name,
                OpenIssues: item.OpenIssues,

                TotalIssues: item.TotalIssues,

                LastTimeShown: item.LastTimeShown
            }
        });
    }

    $scope.refresh = function () {
        
        crudService.get("ValidationPriorities", "List").then(function (data) {

            $scope.priorities = data;
            });

        var loadingRuleParametersPromise = undefined;

        crudService.get("Product", "GetAll").then(function (data) {

            $scope.products = data;
            
            $scope.selectedProduct = data.find(function (item) {
                return item.Id == $scope.productId;
            });

            return $scope.changeForProduct();
        }).then(function () {

            var anchor = window.location.hash;

            if (anchor && anchor !== '') {

                anchor = anchor.slice(1, anchor.length);

                var parts = anchor.split("_");

                var settingId = parts[0];
                var valResultId = parts[1];

                setSelectedValidationSetting(settingId);

                if (!$scope.selectedValidationSetting) {
                    toastr["error"]("Not found such val.setting #"+settingId+"!");
                    return;
                }
                
                if (!loadingRuleParametersPromise) {
                    loadingRuleParametersPromise = loadRuleParameters();
                }

                loadingRuleParametersPromise
                    .then(loadValidationRules)
                    .then($scope.loadXMLData);

                loadSelectedValidationResult(valResultId)
                    .then(function() {

                        if (!$scope.selectedValidationResult) {
                            toastr["error"]("Not found such val.result #" + valResultId + "!");
                            return;
                        }

                        loadXml($scope.selectedValidationResult);
                    });
            }
        });

        if (!loadingRuleParametersPromise) {
            loadingRuleParametersPromise = loadRuleParameters();
        }

        setDefaultDates();
        initSettingsDataTable();
        initCommonValidationResultsDataTable();
        initEventValidationResultsDataTable();
    }

    var loadOperators = function() {
        return crudService.get("ValidationSettings", "GetValidationOperators").then(function (data) {

            $scope.validationOperators = data;
        });
    }

    var loadValidationParameters = function () {
        return crudService.get("ValidationSettings", "GetValidationParameters").then(function (data) {

            $scope.validationParameters = data;
        });
    }
    
    var loadOperatorsToParametersRelations = function () {
        return crudService.get("Product", "GetOperatorsToParametersRelations").then(function (data) {

            $scope.operatorsToParametersRelations = data;
        });
    }

    var loadValidationDataTypes = function () {
        return crudService.get("ValidationSettings", "GetValidationDataTypes")
            .then(function (data) {
                $scope.dataTypes = data;
            });
    }

    var loadRuleParameters = function() {

        return loadOperators()
        .then(loadValidationParameters)
        .then(loadOperatorsToParametersRelations)
        .then(loadValidationDataTypes);
    }

    var setSelectedValidationResult = function(resultId) {
        $scope.selectedValidationResult = $scope.validationResults.find(function (item) {
            return +item.Id === +resultId;
        });
    };

    var setSelectedValidationSetting = function(settingId) {
         $scope.selectedValidationSetting = $scope.validationSettingsModels.find(function (item) {
            return +item.Setting.Id === +settingId;
        });
    }

    var loadSelectedValidationResult = function(resultId)
    {
        return crudService.get("ValidationResults", "GetValidationResult",
            {
                id: resultId
            })
            .then(function (data) {
                $scope.selectedValidationResult = data;
            }); 
    }

    $scope.changeForProduct = function() {

        $rootScope.$broadcast("loading");

        loadProductArrivalMessagesInfo();

        return $scope.loadNextDropdownsByType(0).then(function () {

            return loadValidationSettings();
        });;
    }

    $scope.validationParameters = [];
    $scope.validationOperators = [];
    $scope.dataTypes = [];

    var loadValidationResultsBySetting = function () {

        $("#common_validation_results_modal").modal("show");

        $scope.loadValidationResultsByMultips();
    }

    var setDefaultDates = function () {
        var format = "yyyy-mm-dd hh:ii";
        var viewFormat = "dd/mm/yyyy hh:ii";

        $("#dateFrom").editable({
            format: format,
            viewformat: viewFormat,
            validate: function (value) {
                var valueForParse = $("#dateTo").editable("getValue").dateTo;
                var parsedDateTo = moment(valueForParse, format)._d;

                var parsedDateFrom = new Date(value);

                if (parsedDateFrom >= parsedDateTo) {
                    return 'must be less, then date "To"';
                } else {
                    $timeout(function () {
                        loadValidationSettings();
                    }, 500);
                }
            }
        });

        $("#dateTo").editable({
            format: format,
            viewformat: viewFormat,
            validate: function (value) {
                var valueForParse = $("#dateFrom").editable("getValue").dateFrom;
                var parsedDateFrom = new Date(moment(valueForParse, format)._d);
                var parsedDateTo = new Date(value);
                if (parsedDateTo <= parsedDateFrom) {
                    return 'must be more, then date "From"';
                } else {
                    $timeout(function() {
                            loadValidationSettings();
                        },500);
                }
            }
        });

        var now = new Date();
        $("#dateTo").editable("setValue", now);

        var from = new Date();
        from.setDate(from.getDate() - 7);
        $("#dateFrom").editable("setValue", from);
    }

    var intervalId;

    var loadValidationSettings = function () {

        $rootScope.$broadcast("loading");
        // settingsTable.DataTable().clear().draw();

        $scope.filterObject.StartDate = $("#dateFrom").editable("getValue").dateFrom;
        $scope.filterObject.EndDate = $("#dateTo").editable("getValue").dateTo;

        if (intervalId) {
            $interval.cancel(intervalId);
        }

        if ($scope.updateByInterval) {

            intervalId = $interval(function () {
                loadProductArrivalMessagesInfo();
                loadValidationSettings();
            }, 20000);
        }

        return crudService.post("ValidationResults", "GetValidationSettingsForProduct", {
            id: $scope.selectedProduct ? $scope.selectedProduct.Id : null,
            filter: $scope.filterObject
        }).then(function (data) {
            $scope.validationSettingsModels = data;
            
            $rootScope.$broadcast("loaded");
        }).then(function() {
            $scope.filterSettings();
        });
    }

    var loadProductArrivalMessagesInfo = function() {
        var productId = null;
        if ($scope.selectedProduct && $scope.selectedProduct.Id) {
            productId = $scope.selectedProduct.Id;
        }
        return crudService.post("Product",
                "LastArrivalMessagesInfo",
                {
                    id: productId
                })
            .then(function (data) {
                $scope.arrivalMessagesInfo = data;
            });
    }

    $scope.filter = function () {
        loadValidationSettings();
    }

    $scope.loadValidationResultsByMultips = function () {

        $rootScope.$broadcast("loading");
        commonValidationResultsTable.DataTable().clear().draw();

        return crudService.post("ValidationResults", "GetValidationResultsBySettingId", {
            settingId: $scope.selectedValidationSetting.Setting.Id,
            productId: $scope.selectedValidationSetting.Setting.Product.Id,
            offset: $scope.offset * $scope.counter,
            count: $scope.counter,
            filter: $scope.filterObject,
            loadInactive: $scope.isLoadHistoricalResults
        }).then(function(data) {

            $scope.validationResults = data;

            var dictEvents = {};

            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                var eventId = item.EventId;

                var markets = item.Market && item.Market!="" ? item.Market.split(',') : [];
                var providers = item.Provider && item.Provider != "" ? item.Provider.split(',') : [];

                var dictElem = dictEvents[eventId];
                if (dictElem) {

                    dictElem.Markets.concat(markets);
                    dictElem.Providers.push(providers);
                    dictElem.ValidationResultsCount += 1;

                    if (item.UpdatedOn > item.LastTimeShown) {
                        item.LastTimeShown = item.UpdatedOn
                    }
                } else {
                    dictEvents[eventId] = {
                        Id: item.Id,
                        EventId: item.EventId,
                        EventName: item.EventName,
                        LeagueName: item.LeagueName,
                        StatusName: item.Status,
                        SportName: item.SportName,
                        CountryName: item.LocationName,
                        Markets: markets,
                        Providers: providers,
                        ValidationResultsCount: 1,
                        LastTimeShown: item.UpdatedOn,
                        IsActive: item.IsActive
                    }
                }
            }

            var _dataProvider = [];

            for (var key in dictEvents) {
                var item = dictEvents[key];
                item.MarketsCount = item.Markets.unique().length;
                item.ProvidersCount = item.Providers.unique().length;

                _dataProvider.push(item);
            }

            $scope.commonValidationResults = _dataProvider;

            setDataProvider(commonValidationResultsTable, _dataProvider);

            $rootScope.$broadcast("loaded");
        });
    }

    ////////
    /////

    $.fn.editable.defaults.mode = "popup";
    var original = $.fn.editableutils.setCursorPosition;
    $.fn.editableutils.setCursorPosition = function () {
        try {
            original.apply(this, Array.prototype.slice.call(arguments));
        } catch (e) { /* noop */ }
    };

    $scope.xmlNodes = [];

    $scope.searchText = "";

    $scope.collapseTree = function () {
        showHideTree(false);
    }

    $scope.expandTree = function () {
        showHideTree(true);
    }

    var showHideTree = function (isCollaps) {
        if (isCollaps) {

            tree.jstree("open_all");
        } else {

            tree.jstree("close_all");
        }
    }

    $scope.addNewSetting = function() {
        $("#select-product").show();
    }
    
    $scope.setSelected = function (idSelected) {
        $scope.searchText = "";

        $scope.rulesIndexId = -1;

        rulesIdsForDeleting = [];

        $scope.validationRules = [];

        if (idSelected) {
            setSelectedValidationSetting(idSelected);

            $scope.isShowingRules = false;
            loadValidationRules(false);
        } else {
            $scope.selectedValidationSetting = {
                Setting: {
                    Id: 0,
                    Name: "",
                    Description: "",
                    Expression: ""
                }
            }
        }

        $("#create-edit-validation-settings").modal("show");
    }

    $scope.setProductBySetting = function () {
        if ($scope.selectedValidationSetting) {
            var product = $scope.products.find(function (product) {
                return product.Id == $scope.selectedValidationSetting.Setting.ProductId;
            });

            $scope.selectedProduct = product;
        }
    };

    $scope.wasSelectedProduct = function () {
        
        $scope.setProductBySetting();

        if ($scope.selectedProduct) {
            $scope.selectedValidationSetting.Setting.ProductName = $scope.selectedProduct.Name;
        }

        $scope.isDisabledRules = false;
        $scope.loadXMLData();

        $("#select-product").modal("hide");

        $scope.setSelected();
    }

    var tree = $("#jsTree");
    $scope.lastSelectedNode = "";
    $scope.rulesIndexId = -1;

    $scope.searchInTree = function (event) {
        tree.jstree(true).search(event.target.value);
    }

    $scope.loadXMLData = function () {
        
        crudService.get("Product", "GetXMLData", { productId: $scope.selectedValidationSetting.Setting.ProductId })
            .then(function (data) {
                tree.jstree("destroy");
                data.plugins = ["search"];
                tree.jstree(data);

                tree.on("search.jstree",
                    function (e, data) {
                        if (data.nodes.length) {
                            tree.jstree(true).get_node(data.nodes[0].id, true).children(".jstree-anchor")[0].scrollIntoView();
                        }
                    });

                tree.on("loaded.jstree",
                    function () {
                        showHideTree(true);

                        $scope.xmlNodes = [];

                        $scope.searchText = "";

                        var selfTree = tree.jstree();

                        var index = 1;
                        $(".jstree-leaf")
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
            })
            .then(function() {

                commonService.setEditablesForRules($scope);
            });
    }

    $scope.saveChanges = function () {

        if (isValidatedForm()) {

            var validationSetting = {

                Id: $scope.selectedValidationSetting.Setting.Id,
                Name: $scope.selectedValidationSetting.Setting.Name,
                Description: $scope.selectedValidationSetting.Setting.Description,

                PriorityId: $scope.selectedValidationSetting.Setting.PriorityId,
                ProductId: $scope.selectedProduct.Id,

                ValidationRules: $scope.validationRules,
                Expression: $scope.selectedValidationSetting.Setting.Expression,

                IsSlackEnabled: $scope.selectedValidationSetting.Setting.IsSlackEnabled,
                SlackChannel: $scope.selectedValidationSetting.Setting.SlackChannel
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
        //if ($scope.selectedValidationSetting.Setting.Description == "") {
        //    return false;
        //}

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

    $scope.addRuleToTextArea = function (event, textAreaId, name) {

        var ruleName = " " + name + " ";

        setToTextArea(textAreaId, ruleName, ruleName);
    }

    $scope.addToExpression = function (text, textAreaId) {
        // text = " " + text + " ";
        setToTextArea(textAreaId, "", text);
    }

    function setToTextArea(textAreaId, defaultValue, setValue) {

        var textArea = $("#" + textAreaId);
        var position = textArea.prop("selectionStart");

        $scope.selectedValidationSetting.Setting.Expression = $scope.selectedValidationSetting.Setting.Expression == null ? defaultValue :
                                                        [$scope.selectedValidationSetting.Setting.Expression.slice(0, position), setValue, $scope.selectedValidationSetting.Setting.Expression.slice(position)].join("");
    }
    $("#create-edit-validation-settings").on("shown", function () {
        $("#create-edit-validation-settings input").first().focus();
    });

    $scope.deleteRule = function (ruleId, ruleName) {
        bootbox.confirm('Are you sure you want to delete validation rule "' + ruleName + '"? ', function (result) {
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

                $scope.$digest();
            }
        });
    }

    return vm;
});