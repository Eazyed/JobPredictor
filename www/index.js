// const { default: axios } = require("axios");

var time = "";
var dictionary = {};
var dictionaryPredict = {};
var words = [];

function isNumeric(n) {
  return !isNaN(parseFloat(n)) && isFinite(n);
}

/* Appel vers l'API pour obtenir les données à afficher */

// const axios = require('axios').default;
function getAxios(){
  axios.get('https://localhost:44353/analysis', {
      params: {
      }
    })
    .then(function (response) {
      for (var tmp in response.data){
        // console.log("Analysis ID : " + response.data[tmp].originalId);
        // console.log("Analysis category : " + response.data[tmp].category)
        // console.log("Analysis category predict : " + response.data[tmp].categoryPredict)

        if(!dictionary[response.data[tmp].category]){
          dictionary[response.data[tmp].category] = 1;
        }else{
          dictionary[response.data[tmp].category] += 1;
        }

        if(!dictionaryPredict[response.data[tmp].categoryPredict]){
          dictionaryPredict[response.data[tmp].categoryPredict] = 1;
        }else{
          dictionaryPredict[response.data[tmp].categoryPredict] += 1;
        }

      }

      for (const [keyd, valued] of Object.entries(dictionary)) {
        console.log("Job dictionary : " + keyd + " Value : " + valued);

      axios.get('https://localhost:44353/wordcount', {
        params: {
          category: keyd
        }
      })
      .then(function (response) {
          for (const [key, value] of Object.entries(response.data.weightByWords)) {
            if (value >= 10 && !isNumeric(key)){
              console.log("Line key: " + key, value);
              // window.words[key] = value;
              window.words.push({key: key, value: value, group: keyd});
            }
          }
      })
      .catch(function (error) {
        console.log(error);
      })
      .then(function () {
        // always executed
      });
        // console.log("Wordcount analysis : " + words[0].key);
            // if (!dictionary.find(o => o === response.data[tmp].category)){
            //   dictionary.push(response.data[tmp].category);
            // }
    }
    

  })
  .catch(function (error) {
    console.log(error);
  })
  .then(function () {
    // always executed
  });

  axios.get('https://localhost:44353/lastupdate', {
    params: {
    }
  })
  .then(function (response) {
    window.time = response.data;
  })
  .catch(function (error) {
    console.log(error);
  })
  .then(function () {
    // always executed
  });
}

getAxios();


setTimeout(function() {
/* Plotting results */

        // set the dimensions and margins of the graph
        var margin = {top: 10, right: 10, bottom: 10, left: 10},
            width = 450 - margin.left - margin.right,
            height = 450 - margin.top - margin.bottom;

        // append the svg object to the body of the page
        var svg = d3.select("#my_dataviz").append("svg")
            .attr("width", width + margin.left + margin.right)
            .attr("height", height + margin.top + margin.bottom)
          .append("g")
            .attr("transform",
                  "translate(" + margin.left + "," + margin.top + ")");

        // Constructs a new cloud layout instance. It run an algorithm to find the position of words that suits your requirements
        var layout = d3.layout.cloud()
          .size([width, height])
          .words(window.words.map(function(d) { return {text: d.key}; }))
          .padding(10)
          .fontSize(60)
          .on("end", draw);
        layout.start();

        // This function takes the output of 'layout' above and draw the words
        // Better not to touch it. To change parameters, play with the 'layout' variable above
        function draw(words) {
          svg
            .append("g")
              .attr("transform", "translate(" + layout.size()[0] / 2 + "," + layout.size()[1] / 2 + ")")
              .selectAll("text")
                .data(words)
              .enter().append("text")
                .style("font-size", function(d) { return d.size + "px"; })
                .attr("text-anchor", "middle")
                .attr("transform", function(d) {
                  return "translate(" + [d.x, d.y] + ")rotate(" + d.rotate + ")";
                })
                .text(function(d) { return d.text; });
        }

        /* -------------------------------------- 4ème visualisation ---------------------------------*/


        // set the dimensions and margins of the graph
        var margin = {top: 50, right: 40, bottom: 50, left: 65},
          width = 1450 - margin.left - margin.right,
          height = 450 - margin.top - margin.bottom;

        // append the svg object to the body of the page
        var svg = d3.select("#my_datavizfour")
        .append("svg")
          .attr("width", width + margin.left + margin.right)
          .attr("height", height + margin.top + margin.bottom)
        .append("g")
          .attr("transform",
                "translate(" + margin.left + "," + margin.top + ")");

          // Labels of row and columns -> unique identifier of the column called 'group' and 'variable'
          var myGroups = d3.map(words, function(d){return d.key;}).keys()
          var myVars = d3.map(dictionary, function(d){return d;}).keys()

          // Build X scales and axis:
          var x = d3.scaleBand()
            .range([ 0, width ])
            .domain(myGroups)
            .padding(0.05);
          svg.append("g")
            .style("font-size", 15)
            .attr("transform", "translate(0," + height + ")")
            .call(d3.axisBottom(x).tickSize(0))
            .selectAll("text")  
            .style("text-anchor", "end")
            .attr("dx", "-.8em")
            .attr("dy", ".15em")
            .attr("transform", "rotate(-65)" )
            .select(".domain").remove()

          // Build Y scales and axis:
          var y = d3.scaleBand()
            .range([ height, 0 ])
            .domain(myVars)
            .padding(0.05);
          svg.append("g")
            .style("font-size", 15)
            .call(d3.axisLeft(y).tickSize(0))
            .select(".domain").remove()

          // Build color scale
          var myColor = d3.scaleSequential()
            .interpolator(d3.interpolateInferno)
            .domain([1,100])

          // create a tooltip
          var tooltip = d3.select("#my_datavizfive")
            .append("div")
            .style("opacity", 0)
            .attr("class", "tooltip")
            .style("background-color", "white")
            .style("border", "solid")
            .style("border-width", "2px")
            .style("border-radius", "5px")
            .style("padding", "5px")

          // Three function that change the tooltip when user hover / move / leave a cell
          var mouseover = function(d) {
            tooltipd
              .transition()
              .duration(200)
              .style("opacity", 1)
            tooltipd
                .html("<span style='color:grey'>" + d.key.replace("_", " ").replace(/^./, d.key[0].toUpperCase()) + "<br/>Nombre d'occurences : </span>" + d.value)
                .style("left", (event.pageY) + "px")
                .style("top", (event.pageX) + "px")
            }
            var mousemove = function(d) {
            tooltipd
                .style("left", (event.pageX) + "px")
                .style("top", (event.pageY) + "px")
            }
            var mouseleave = function(d) {
            tooltipd
              .transition()
              .duration(200)
              .style("opacity", 0)
            }

          // add the squares
          svg.selectAll()
            .data(words, function(d) {return d.key+':'+d.value;})
            .enter()
            .append("rect")
              .attr("x", function(d) { return x(d.key) })
              .attr("y", function(d) { return y(d.group) })
              .attr("rx", 4)
              .attr("ry", 4)
              .attr("width", x.bandwidth() )
              .attr("height", y.bandwidth() )
              .style("fill", function(d) { return myColor(d.value)} )
              .style("stroke-width", 4)
              .style("stroke", "none")
              .style("opacity", 0.8)
            .on("mouseover", mouseover)
            .on("mousemove", mousemove)
            .on("mouseleave", mouseleave)

/* ------------------------- 2ème visualisation --------------------------------- */

// Create a tooltip
var tooltip = d3.select("#my_dataviztwo")
.append("div")
  .style("opacity", 0)
  .attr("class", "tooltip")
  .style("font-size", "16px")
// Three function that change the tooltip when user hover / move / leave a cell
var mouseover = function(d) {
tooltip
  .transition()
  .duration(200)
  .style("opacity", 1)
tooltip
    .html("<span style='color:grey'>" + d.data.key.replace("_", " ").replace(/^./, d.data.key[0].toUpperCase()) + "<br/>Nombre d'occurences : </span>" + d.value)
    .style("left", (event.pageX) + "px")
    .style("top", (event.pageY) + "px")
}
var mousemove = function(d) {
tooltip
  .style("left", (event.pageX) + "px")
  .style("top", (event.pageY) + "px")
}
var mouseleave = function(d) {
tooltip
  .transition()
  .duration(200)
  .style("opacity", 0)
}

 // set the dimensions and margins of the graph
var width = 450
height = 450
margin = 40

// The radius of the pieplot is half the width or half the height (smallest one). I subtract a bit of margin.
var radius = Math.min(width, height) / 2 - margin

// append the svg object to the div called 'my_dataviz'
var svg = d3.select("#my_dataviztwo")
.append("svg")
.attr("width", width)
.attr("height", height)
.append("g")
.attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

// set the color scale
var color = d3.scaleOrdinal()
.domain(dictionary)
.range(d3.schemeSet3);

// Compute the position of each group on the pie:
var pie = d3.pie()
.value(function(d) {return d.value; })
var data_ready = pie(d3.entries(dictionary))
// Now I know that group A goes from 0 degrees to x degrees and so on.

// shape helper to build arcs:
var arcGenerator = d3.arc()
.innerRadius(0)
.outerRadius(radius)

// Build the pie chart: Basically, each part of the pie is a path that we build using the arc function.
svg
.selectAll('mySlices')
.data(data_ready)
.enter()
.append('path')
.attr('d', arcGenerator)
.attr('fill', function(d){ return(color(d.data.key)) })
.attr("stroke", "black")
.style("stroke-width", "2px")
.style("opacity", 0.7)
.on("mouseover", mouseover)
.on("mousemove", mousemove)
.on("mouseleave", mouseleave)


/* ---------------------------------------- 3ème visualisation ------------------------- */

// Create a tooltip
var tooltipd = d3.select("#my_datavizthree")
.append("div")
  .style("opacity", 0)
  .attr("class", "tooltip")
  .style("font-size", "16px")
// Three function that change the tooltip when user hover / move / leave a cell
var mouseover = function(d) {
tooltipd
  .transition()
  .duration(200)
  .style("opacity", 1)
tooltipd
    .html("<span style='color:grey'>" + d.data.key.replace("_", " ").replace(/^./, d.data.key[0].toUpperCase()) + "<br/>Nombre d'occurences : </span>" + d.value)
    .style("left", (event.pageY) + "px")
    .style("top", (event.pageX) + "px")
}
var mousemove = function(d) {
tooltipd
    .style("left", (event.pageX) + "px")
    .style("top", (event.pageY) + "px")
}
var mouseleave = function(d) {
tooltipd
  .transition()
  .duration(200)
  .style("opacity", 0)
}

 // set the dimensions and margins of the graph
var width = 450
height = 450
margin = 40

// The radius of the pieplot is half the width or half the height (smallest one). I subtract a bit of margin.
var radius = Math.min(width, height) / 2 - margin

// append the svg object to the div called 'my_dataviz'
var svg = d3.select("#my_datavizthree")
.append("svg")
.attr("width", width)
.attr("height", height)
.append("g")
.attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

// set the color scale
var color = d3.scaleOrdinal()
.domain(dictionaryPredict)
.range(d3.schemeSet3);

// Compute the position of each group on the pie:
var pie = d3.pie()
.value(function(d) {return d.value; })
var data_ready = pie(d3.entries(dictionaryPredict))
// Now I know that group A goes from 0 degrees to x degrees and so on.

// shape helper to build arcs:
var arcGenerator = d3.arc()
.innerRadius(0)
.outerRadius(radius)

// Build the pie chart: Basically, each part of the pie is a path that we build using the arc function.
svg
.selectAll('mySlices')
.data(data_ready)
.enter()
.append('path')
.attr('d', arcGenerator)
.attr('fill', function(d){ return(color(d.data.key)) })
.attr("stroke", "black")
.style("stroke-width", "2px")
.style("opacity", 0.7)
.on("mouseover", mouseover)
.on("mousemove", mousemove)
.on("mouseleave", mouseleave)

console.log(time);
time = time.replace("T", " à ");
time = time.replace("Z", "");
var nav = document.getElementById("imported");
nav.innerHTML += `<a class="nav-link disabled" href="#" tabindex="-1" aria-disabled="true">Date du dernier fichier importé : ` + time + `</a>`;

}, 1000);


function update(){
  axios.get('https://localhost:44353/update', {
    params: {
    }
  })
  .then(function (response) {
    var nav = document.getElementById("imported");
    nav.innerHTML += `<a class="nav-link disabled" href="#" tabindex="-1" aria-disabled="true">Import effectué</a>`;
  })
}
