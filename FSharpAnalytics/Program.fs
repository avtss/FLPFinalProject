open System
open Npgsql.FSharp

let connStr = "Host=localhost;Port=5432;Database=Akinator;Username=postgres;Password=123"

let getTopActiveUsers () =
    Sql.connect connStr
    |> Sql.query "
        SELECT u.username, COUNT(gs.session_id) AS session_count
        FROM users u
        LEFT JOIN sessions gs ON gs.user_id = u.user_id
        GROUP BY u.username
        ORDER BY session_count DESC
        LIMIT 10;"
    |> Sql.execute (fun read -> read.string "username", read.int "session_count")

let getUsersWithCharacterCounts () =
    Sql.connect connStr
    |> Sql.query "
        SELECT u.user_id, u.username, COUNT(c.character_id) AS characters_added
        FROM users u
        LEFT JOIN characters c ON u.user_id = c.added_by_user_id
        GROUP BY u.user_id, u.username;"
    |> Sql.execute (fun read -> read.int "user_id", read.string "username", read.int "characters_added")

[<EntryPoint>]
let main argv =
    printfn "Top active users by sessions:"
    getTopActiveUsers()
    |> List.iter (fun (username, count) -> printfn "%s: %d" username count)

    printfn "\nUsers with added characters count:"
    getUsersWithCharacterCounts()
    |> List.iter (fun (userId, username, count) -> printfn "User %d (%s): %d characters added" userId username count)

    0
