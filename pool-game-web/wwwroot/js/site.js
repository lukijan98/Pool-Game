// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// import {signalR} from "~/js/signalr/dist/browser/signalr";
$(()=>{
    LoadReservationData();
    var connection = new signalR.HubConnectionBuilder().withUrl("/signalrServer").build();
    connection.start();
    connection.on("LoadReservations",function(){
        LoadReservationData();
    })

    LoadReservationData();

    function LoadReservationData(){
        var tr ="";
        $.ajax({
            url:'/Reservations/GetReservations',
            method: 'GET',
            //dataType : "html",
            success:(result)=>{
                console.log(result);
                //console.log($(result).find('#table'));
                // var wrapper=document.createElement('div');
                // wrapper.innerHTML=result;
                 //var t = result.getElementById("table");
                // var tr = t.getElementsByTagName("tr");
                // for (let i = 1; i < tr.length; i++) {
                //     var td = tr[i].getElementsByTagName("td");
                //     final+=`<tr>
                //                 `;
                //     for (let j = 0; j < td.length;j++){
                //         final+=`<td>`+td[j].innerText+`</td>
                //                 `;
                //     }
                //     final+='</tr>';
                //   } 
                //   console.log(final);
                // console.log(tr.getElementsByTagName("td")[0].innerText);
                // console.log(tr.getElementsByTagName("td")[1].innerText);
                $.each(result,(k,v)=>{
                    tr+=`<tr>
                        <td>${v.reservationName}</td>
                        <td>${v.date}</td>
                        <td>${v.timeStart}</td>
                        <td>${v.timeEnding}</td>
                        <td>${v.identityUser.email}</td>
                        <td>${v.poolTable.poolTableId}</td>
                        <td>
                            <a href='../Reservations/Edit?id=${v.reservationId}'>Edit</a>
                            <a href='../Reservations/Details?id=${v.reservationId}'>Details</a>
                            <a href='../Reservations/Delete?id=${v.reservationId}'>Delete</a>
                        </td>
                </tr>`
                })
                $("#tableBody").empty().html(tr);
            },
            error:(error)=>{
                console.log(error)
            }
        });
    }
}
)