namespace FSharpAnalytics

module Queries =
    open System
    open Npgsql.FSharp

    let getTopActiveUsers connStr =
        Sql.connect connStr
        |> Sql.query "
            SELECT u.username, COUNT(s.session_id) AS session_count
            FROM users u
            LEFT JOIN sessions s ON s.user_id = u.user_id
            GROUP BY u.username
            ORDER BY session_count DESC
            LIMIT 10;"
        |> Sql.execute (fun read -> read.string "username", read.int "session_count")

    let getUsersWithCharacterCounts connStr =
        // В новой схеме нет таблицы characters, но есть AddedGames, 
        // считаем количество добавленных игр каждым пользователем
        Sql.connect connStr
        |> Sql.query "
            SELECT u.user_id, u.username, COUNT(ag.added_game_id) AS characters_added
            FROM users u
            LEFT JOIN added_games ag ON u.user_id = ag.added_by_user_id
            GROUP BY u.user_id, u.username;"
        |> Sql.execute (fun read -> 
            read.int "user_id", 
            read.string "username", 
            read.int "characters_added")

    let getUsersWithQuestionsCount connStr =
        // теперь вопросы в AddedQuestions, считаем сколько добавил каждый пользователь
        Sql.connect connStr
        |> Sql.query "
            SELECT u.user_id, u.username, COUNT(aq.added_question_id) AS questions_added
            FROM users u
            LEFT JOIN added_questions aq ON u.user_id = aq.added_by_user_id
            GROUP BY u.user_id, u.username
            ORDER BY questions_added DESC;"
        |> Sql.execute (fun read -> 
            read.int "user_id", 
            read.string "username", 
            read.int "questions_added")

    let getQuestionsWithAuthors connStr =
        Sql.connect connStr
        |> Sql.query "
            SELECT aq.added_question_id AS question_id, aq.text, u.username AS added_by
            FROM added_questions aq
            LEFT JOIN users u ON aq.added_by_user_id = u.user_id
            ORDER BY aq.added_question_id;"
        |> Sql.execute (fun read -> 
            read.int "question_id", 
            read.string "text", 
            read.string "added_by")

    // Популярные ответы по каждому вопросу (на основе позиции ответа в массиве answers)
    let getPopularAnswersByPosition connStr =
        Sql.connect connStr
        |> Sql.query "
            SELECT
                ag.session_id,
                ag.user_id,
                string_to_array(ag.answers, ',') AS answers_array
            FROM alias_games ag;"
        |> Sql.execute (fun read ->
            read.int "session_id",
            read.int "user_id",
            read.stringArray "answers_array")


    let getPopularAnswersByQuestion connStr =
        let sessions = getPopularAnswersByPosition connStr

        let maxLength =
            sessions |> Seq.map (fun (_, _, answers) -> answers.Length) |> Seq.max

        [0 .. maxLength - 1]
        |> List.collect (fun pos ->
            sessions
            |> Seq.choose (fun (_, _, answers) ->
                if pos < answers.Length then Some (answers[pos].Trim().ToLower())
                else None)
            |> Seq.countBy id
            |> Seq.map (fun (answer, count) -> (pos + 1, $"Question #{pos + 1}", answer, count))
            |> Seq.toList)

