
(function ($) {
    "use strict";


    /*==================================================================
    [ Focus Contact2 ]*/
    $('.input100').each(function () {
        $(this).on('blur', function () {
            if ($(this).val().trim() != "") {
                $(this).addClass('has-val');
            }
            else {
                $(this).removeClass('has-val');
            }
        })
    })


    /*==================================================================
    [ Validate ]*/
    var input = $('.validate-input .input100');

    $('.validate-form').on('submit', function () {
        var check = true;

        for (var i = 0; i < input.length; i++) {
            if (validate(input[i]) == false) {
                showValidate(input[i]);
                check = false;
            }
        }

        return check;
    });


    $('.validate-form .input100').each(function () {
        $(this).focus(function () {
            hideValidate(this);
        });
    });

    function validate(input) {
        if ($(input).attr('type') == 'email' || $(input).attr('name') == 'email') {
            if ($(input).val().trim().match(/^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{1,5}|[0-9]{1,3})(\]?)$/) == null) {
                return false;
            }
        }
        else {
            if ($(input).val().trim() == '') {
                return false;
            }
        }
    }

    function showValidate(input) {
        var thisAlert = $(input).parent();

        $(thisAlert).addClass('alert-validate');
    }

    function hideValidate(input) {
        var thisAlert = $(input).parent();

        $(thisAlert).removeClass('alert-validate');
    }


})(jQuery);
$(document).ready(function () {
    $(window).on('scroll', function () {
        if ($(window).scrollTop() < 1000) {
            $('.hero').css('background-size', 130 + parseInt($(window).scrollTop() / 5) + '%');
        }
    }
        );
});
var groupMins = document.getElementById('minutes'),
    groupHours = document.getElementById('hours'),
    minsSvg = document.getElementsByClassName('min'),
    hoursSvg = document.getElementsByClassName('hour'),
    weekDay = document.getElementsByClassName('week-day'),
    day = document.getElementById('day'),
    hoursSvgArr = [],
    minsSvgArr = [],
    arrWeekDay = [],
    createSvg, polarToCortesian, describeSector,
    sec = 0,
    addClass,
    arc, min, hour, updateClock, getNewArray, updateTimeClock;

createSvg = function (type, location, amount, distance, height, width, y, x, cls) {
    var angle = 0;
    for (var i = 0; i < amount; i++) {
        angle += distance;
        var svg = document.createElementNS("http://www.w3.org/2000/svg", type);
        svg.setAttribute("height", height);
        svg.setAttribute("width", width);
        svg.setAttribute("y", y);
        svg.setAttribute("x", x);
        svg.setAttribute("class", cls);
        svg.setAttribute("transform", `rotate(${angle})`);
        location.appendChild(svg);
    }
}

polarToCortesian = function (centerX, centerY, radius, angleInDegrees) {
    var angleInRadians = (angleInDegrees - 90) * Math.PI / 180.00;

    return {
        x: centerX + (radius * Math.cos(angleInRadians)),
        y: centerY + (radius * Math.sin(angleInRadians))
    }
};

describeSector = function (x, y, radius, startAngle, endAngle) {
    var start = polarToCortesian(x, y, radius, endAngle),
        end = polarToCortesian(x, y, radius, startAngle)

    var largeArcFlag = endAngle - startAngle <= 180 ? "0" : "1";

    d = [
        "M", start.x, start.y,
        "A", radius, radius, 0, largeArcFlag, 0, end.x, end.y,
    ].join(' ')
    return d;
};

addClass = function (num, arr) {
    var i;
    for (i = 0; i < num; i++) {
        arr[i].setAttribute("class", "justdoit");
    }
};

getNewArray = function (arr, location) {
    Array.from(arr).forEach(function (item) {
        location.push(item);
    })
};

updateTimeClock = function () {
    var date = new Date(),

        h = date.getHours().toString(),
        m = date.getMinutes().toString(),
        s = date.getSeconds().toString();
    day = date.getDay()

    if (h.length < 2) {
        h = '0' + h; //two values
    }
    if (m.length < 2) {
        m = '0' + m; //two values
    }
    if (s.length < 2) {
        s = '0' + s; //two values
    }

    document.getElementById("h").innerHTML = h;
    document.getElementById("m").innerHTML = m;
    document.getElementById("s").innerHTML = s;

    arrWeekDay[day].setAttribute("class", "week-active")
}

updateClock = function () {

    var date = new Date(),
        s = date.getSeconds(),
        m = date.getMinutes(),
        h = (date.getHours() + 24) % 12 || 12,
        d = document.getElementById("day"),
        y = document.getElementById("year"),
        setClass = "class",
        classAction = "justdoit",
        classDefault = "min",
        months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    document.getElementById("month").innerHTML = months[date.getMonth()];

    d.textContent = date.getDate()
    y.textContent = date.getFullYear()

    //seconds
    if (sec == 354) {
        sec = 359.994;
    } else if (sec == 359.994) {
        sec = s * 6;
    } else {
        sec = s * 6;
    };

    arc = document.getElementById('seconds').setAttribute('d', describeSector(100, 100, 120, 0, sec));

    //minutes
    if (min === 0) {
        for (var i = 0; i < minsSvgArr.length; i++) {
            minsSvgArr[i].setAttribute(setClass, classAction);
        }
        min = m;
    } else if (min > 0) {
        for (var i = 0; i < minsSvgArr.length; i++) {
            minsSvgArr[i].setAttribute(setClass, classDefault);
        };
        min = m;
        addClass(min, minsSvgArr);
    } else {
        min = m;
        addClass(min, minsSvgArr);
    };

    //hours
    if (hour === 0) {
        for (var i = 0; i < hoursSvgArr.length; i++) {
            hoursSvgArr[i].setAttribute(setClass, classAction);
        }
        hour = h;
    } else if (hour > 0) {
        for (var i = 0; i < hoursSvgArr.length; i++) {
            hoursSvgArr[i].setAttribute(setClass, classDefault);
        }
        hour = h;
        addClass(hour, hoursSvgArr);
    } else {
        hour = h;
        addClass(hour, hoursSvgArr);
    };
    updateTimeClock();
};

setInterval(updateClock, 1000) // Update clock every second
createSvg('rect', groupMins, 60, 6, 15, 5, -110, -5, "min"); // create SVG minutes
createSvg('rect', groupHours, 12, 30, 15, 5, -55, -5, "hour"); // create SVG hours

getNewArray(minsSvg, minsSvgArr);
getNewArray(hoursSvg, hoursSvgArr);
getNewArray(weekDay, arrWeekDay);