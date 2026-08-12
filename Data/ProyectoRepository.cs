using System.Collections.Generic;
using Dapper;
using ScrumMvp.Models;

namespace ScrumMvp.Data
{
    public class ProyectoRepository
    {
        // HU-013/014/015/016: crea el proyecto y devuelve el Id generado.
        public int Crear(string nombre, string productGoal, string vision, string descripcion, int creadoPor)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    INSERT INTO proyecto (nombre, product_goal, vision, descripcion, creado_por)
                    VALUES (@nombre, @productGoal, @vision, @descripcion, @creadoPor);
                    SELECT LAST_INSERT_ID();";

                return con.ExecuteScalar<int>(sql, new { nombre, productGoal, vision, descripcion, creadoPor });
            }
        }

        // Se usa para mostrar los proyectos donde el usuario participa
        // (por ahora, simplificado: los que él mismo creó).
        public List<Proyecto> ObtenerPorCreador(int usuarioId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id           AS Id,
                           nombre       AS Nombre,
                           product_goal AS ProductGoal,
                           vision       AS Vision,
                           descripcion  AS Descripcion,
                           creado_por   AS CreadoPor
                    FROM proyecto
                    WHERE creado_por = @usuarioId
                    ORDER BY id DESC";

                return new List<Proyecto>(con.Query<Proyecto>(sql, new { usuarioId }));
            }
        }

        public Proyecto ObtenerPorId(int id)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id           AS Id,
                           nombre       AS Nombre,
                           product_goal AS ProductGoal,
                           vision       AS Vision,
                           descripcion  AS Descripcion,
                           creado_por   AS CreadoPor
                    FROM proyecto
                    WHERE id = @id";

                return con.QueryFirstOrDefault<Proyecto>(sql, new { id });
            }
        }
    }
}
