  function Dijkstra(Graph, source):
      
      for each vertex v in Graph.Vertices:
          dist[v] ← INFINITY
          prev[v] ← UNDEFINED
          add v to Q
      dist[source] ← 0       
       while Q is not empty:
          u ← vertex in Q with min dist[u]
          remove u from Q
          
          for each neighbor v of u still in Q:
              alt ← dist[u] + Graph.Edges(u, v)
              if alt < dist[v]:
                  dist[v] ← alt
                  prev[v] ← u

      return dist[], prev[]


    function make_noise_grid(density):
        noise_grid ← [map.height][map.width]
        random ← random(1, 100)
        for each map tile:
            if(random > density)
                noise_grid[i][j] = tile.floor
            else
                noise_grid[i][j] = tile.wall
        return noise_grid


    function apply_cellular_automaton(grid, count)
        for(i = 1... count)
        var temp_grid = grid
            for(j = 1 .. map.height)
                for(k = 1 ... map.width)
                var neighbor_wall_count = 0
                    for(y = j - 1 .. j + 1)
                        for(x = k - 1.. k + 1)
                            if(is_within_map_bounds(x, y))
                                if(y != j or x != k)
                                    if(temp_grid[y][x] == tile.wall)
                                        neighbor_wall_count++
                            else
                                neighbor_wall_count++;
            if (neighbor_wall_count > 4)
                grid[j][k] = tile.wall
            else
                grid[j][k] = tile.floor

    procedure check_available_checkpoints
        while(discovered_checkpoints != total_checkpoints)
            apply_cellular_automaton(grid, count)
        end procedure