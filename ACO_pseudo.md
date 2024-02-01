procedure ACO meta-heuristic()
while (termination criterion not satisfied)
Start Schedule_activities
ants generation and activity();
pheromone evaporation();
daemon actions(); {optional}
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
while (current state ̸= target state)
A = read local ant-routing table();
P = compute transition probabilities(A, M, Ω);
next state = apply ant decision policy(P,Ω);
move to next state(next state);
if (online step-by-step pheromone update)
depost_pheromone_on_the_visted_arc();
update_ant_rounting_table();
End if
M = update_internal_state();
End while
if (online_step-by-step_pheromone_update)
Foreach visited_arc ∈ ψ do
deposit pheromone on the visited arc();
Update ant routing table();
end foreach
end if
die();
end procedure
