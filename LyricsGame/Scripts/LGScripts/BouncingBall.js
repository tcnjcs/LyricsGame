var segcount = 0;
var lowerBound = -1;
var upperBound = -1;
var count = 0;
var atime = -1;
$(document).ready(function () {
    //$("#segrow td:first()").css("background-color", "SandyBrown");
       
    $("#player").bind("play", function () {
        if (!(Math.floor(this.currentTime) == 0)) { return; }
        $("#segtable tr td:nth-child(1)").css("background-color", "SandyBrown");
    });
    $("#player").bind("timeupdate", function () {
        var audio = this;
        atime = audio.currentTime;
          
        if (lowerBound == -1) { lowerBound = parseInt($("#segrow td:nth-child(1)").text()); }
        if (upperBound == -1) { upperBound = parseInt($("#segrow td:nth-child(2)").text()); }

        //console.log(lowerBound + ", " + atime + ", " + upperBound);
         
        if (atime < lowerBound) {
            console.log("lower broached");
            $($(".segtime").get().reverse()).each(function () {
                if (atime > parseInt(this.textContent)) {
                    upperBound = parseInt($(this).next().text());
                    lowerBound = parseInt($(this).text());
                    count++;
                    //console.log(count);
                    var dontclear = $(this).index() + 1;
                    $("#segtable tr td:nth-child(" + dontclear + ")").css("background-color", "SandyBrown");
                    $("#segtable tr td:not(:nth-child(" + dontclear + "))").css("background-color", "inherit");
                    return false;
                }
            });
        }

        if (atime > upperBound) {
            //console.log("upper broached");
            $(".segtime").each(function () {
                if (atime < parseInt(this.textContent)) {
                    upperBound = parseInt($(this).text());
                    lowerBound = parseInt($(this).prev().text());
                    count++;
                    //console.log(count);
                    var dontclear = $(this).index();
                    $("#segtable tr td:nth-child(" + dontclear + ")").css("background-color", "SandyBrown");
                    $("#segtable tr td:not(:nth-child(" + dontclear + "))").css("background-color", "inherit");

                    return false;
                }
            });
        }
    });
});

