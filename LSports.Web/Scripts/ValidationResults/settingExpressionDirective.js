validationResultsApp.directive("settingexpressionDirective", function ($timeout, $rootScope, crudService, $compile) {

    var template = '<div style="position: relative;"><div id="{{id}}">{{expression}}</div><div style="position: absolute;right: 0px;top: 0px;" ng-click="clickOnShowRulesTable()" class="bordered-image" ><img style="width: 20px;" src="data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iaXNvLTg4NTktMSI/Pg0KPCEtLSBHZW5lcmF0b3I6IEFkb2JlIElsbHVzdHJhdG9yIDE2LjAuMCwgU1ZHIEV4cG9ydCBQbHVnLUluIC4gU1ZHIFZlcnNpb246IDYuMDAgQnVpbGQgMCkgIC0tPg0KPCFET0NUWVBFIHN2ZyBQVUJMSUMgIi0vL1czQy8vRFREIFNWRyAxLjEvL0VOIiAiaHR0cDovL3d3dy53My5vcmcvR3JhcGhpY3MvU1ZHLzEuMS9EVEQvc3ZnMTEuZHRkIj4NCjxzdmcgdmVyc2lvbj0iMS4xIiBpZD0iQ2FwYV8xIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHhtbG5zOnhsaW5rPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5L3hsaW5rIiB4PSIwcHgiIHk9IjBweCINCgkgd2lkdGg9IjQwNC4zMDhweCIgaGVpZ2h0PSI0MDQuMzA5cHgiIHZpZXdCb3g9IjAgMCA0MDQuMzA4IDQwNC4zMDkiIHN0eWxlPSJlbmFibGUtYmFja2dyb3VuZDpuZXcgMCAwIDQwNC4zMDggNDA0LjMwOTsiDQoJIHhtbDpzcGFjZT0icHJlc2VydmUiPg0KPGc+DQoJPHBhdGggZD0iTTAsMTAxLjA4aDQwNC4zMDhMMjAyLjE1MSwzMDMuMjI5TDAsMTAxLjA4eiIvPg0KPC9nPg0KPGc+DQo8L2c+DQo8Zz4NCjwvZz4NCjxnPg0KPC9nPg0KPGc+DQo8L2c+DQo8Zz4NCjwvZz4NCjxnPg0KPC9nPg0KPGc+DQo8L2c+DQo8Zz4NCjwvZz4NCjxnPg0KPC9nPg0KPGc+DQo8L2c+DQo8Zz4NCjwvZz4NCjxnPg0KPC9nPg0KPGc+DQo8L2c+DQo8Zz4NCjwvZz4NCjxnPg0KPC9nPg0KPC9zdmc+DQo="/></div></div>';

    function showTooltip(x, y, content) {
        $('<div id="tooltip">' + content + '</div>').css({
            position: 'absolute',
            display: 'none',
            'z-index': 100000,
            top: y + 15,
            left: x + 15,
            border: '1px solid #333',
            padding: '10px 20px 10px 20px',
            'border-radius': '3px',
            'background-color': 'white',
        }).appendTo("body").fadeIn(200);
    }

    function clickOnShowRulesTable() {
        $rootScope.$broadcast("showRulesTable");
    }

    function buildRulesLinks(scope, templateInDoc) {
    	var html = "";

        if (scope.expression) {

            html = templateInDoc.innerHTML;
        } else {

            var tmpArray = [];
            for (var i = 0; i < scope.rules.length; i++) {
                tmpArray.push(scope.rules[i].RuleName + " ");
            }

            html = tmpArray.join("AND ");
        }

        for (var i = 0; i < scope.rules.length; i++) {
            var ruleName = scope.rules[i].RuleName;
            var ruleId = scope.rules[i].Id;

            var reg_1 = new RegExp(ruleName, 'g');

            var match_1 = html.match(reg_1);
            if (match_1) {

                html = html.replace(reg_1,
                     '<a style="cursor: pointer" id="' + ruleId + '" class="rulesInExpression">' + ruleName + '</a>');
            } 

        }

        templateInDoc.innerHTML = html;
    }


    function bindRulesToClick(scope) {

        $(".rulesInExpression").bind('click', function (event) {
            $("#tooltip").remove();

            var ruleId = this.getAttribute("id");

            var rule = scope.rules.find(function (rule) {
                return rule.Id == ruleId;
            });

            var content = rule.PropertyName + "    " + rule.Parameter.Name + "    " + rule.Operator.Name + "    " + rule.Value;

            showTooltip(event.pageX, event.pageY, content);

            $("#tooltip").bind('mouseleave', function (event) {
                $("#tooltip").hide('slow');
            });
        });

    }

	function redrawRules(scope) {
		$timeout(function () {
			var templateInDoc = $('#' + scope.id)[0];

			if (!templateInDoc) {
				return;
			}

			buildRulesLinks(scope, templateInDoc);

			bindRulesToClick(scope);
		},
		300);
	};

    return {
        restrict: 'AE',
        scope: {
            id: '=',
            expression: '=',
            rules: '='
        },
        transclude: true, 
        template: template,
        link: function (scope, element, attrs) {

            scope.clickOnShowRulesTable = clickOnShowRulesTable;

            scope.$watch('expression', function () {
                element.html(template);
                $compile(element.contents())(scope);
                redrawRules(scope);
            });

            scope.$watch('rules', function (newRules) {

                scope.rules = newRules;

	            redrawRules(scope);
            });

        }
    };
});