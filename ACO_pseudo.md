procedure ACO meta-heuristic()
    while (termination criterion not satisfied)
        Start Schedule_activities
            ants_generation_and_activity();
            pheromone_evaporation();
            daemon_actions(); {optional}
        end schedule_activities
    end while
end procedure




procedure ants_generation_and_activity()
    while (available_resources)
        schedule_the_creation_of_a_new_ant();
        new_active_ant();
    end while
end procedure




procedure new_active_ant() {ant lifecycle}
    initialize ant();
    M = update ant memory();
    while (current state /= target state)
        A = read_local_ant-routing_table();
        P = compute transition probabilities(A, M, Ω);
        next_state = apply_ant_decision_policy(P,Ω);
        move_to_next_state(next state);
        if (online_step-by-step_pheromone_update)
            depost_pheromone_on_the_visted_arc();
            update_ant_rounting_table();
        End if
        M = update_internal_state();
    End while
    if (online_step-by-step_pheromone_update)
        Foreach_visited_arc ∈ ψ do
            deposit_pheromone_on_the_visited_arc();
            Update_ant_routing_table();
        end foreach
    end if
    die();
end procedure
