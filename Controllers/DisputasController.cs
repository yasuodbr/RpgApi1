using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgApi.Data;
using RpgApi.Models;

namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DisputasController : ControllerBase
    {
        private readonly DataContext _context;


    public DisputasController(DataContext context)
        {
             _context=context;  
        }


        [HttpPost("Arma")]

 public async Task<IActionResult> AtaqueComArmaAsync(Disputa d)
        {
            try
            {

                Personagem? atacante = await _context.TB_PERSONAGENS
                .Include(p =>p.Arma)
                .FirstOrDefaultAsync(p => p .Id ==d.AtacanteId);

                Personagem? oponente = await _context.TB_PERSONAGENS
                .FirstOrDefaultAsync(p => p.Id == d.OponenteId);


                int dano = atacante.Arma.Dano + (new Random().Next(atacante.Forca));

                dano = dano - new Random().Next(oponente.Defesa);

                if (dano >  0)
                        oponente.PontosVida = oponente.PontosVida - (int)dano;
                if (oponente.PontosVida < 0 )
                        d.Narracao = $"{oponente.Nome}FOI DERROTADO";
                        _context.TB_PERSONAGENS.Update(oponente);
                        await _context.SaveChangesAsync();


                StringBuilder dado = new StringBuilder();

                dado.AppendFormat("Atacante: {0}. ",atacante.Nome);
                dado.AppendFormat("Oponente: {0}. ", oponente.Nome);
                dado.AppendFormat("Ponto de vida do atacante: {0}. ", atacante.PontosVida);
                dado.AppendFormat("Ponto de vida do oponente: {0}. ",oponente.PontosVida);
                dado.AppendFormat("Arma utilizada: {0}. ",atacante.Arma.Nome);
                dado.AppendFormat("Dano: {0}. ", dano);

                d.Narracao +=dado.ToString();
                d.DataDisputa = DateTime.Now;
                _context.TB_DISPUTA.Add(d);
                _context .SaveChanges();

                return Ok(d);
            }
            catch(System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Habilidade")]

        public async Task<IActionResult> AtaqueComHabilidadeAsync(Disputa d )
        {
            try
            {
                return Ok(d);
            }
            catch (System.Exception ex)
            {

                Personagem atacante = await _context.TB_PERSONAGENS
                .Include(p =>p.PersonagemHabilidades).ThenInclude(ph => ph.Habilidade)
                .FirstOrDefaultAsync(p => p.Id == d.AtacanteId);

                Personagem oponente = await _context.TB_PERSONAGENS
                .FirstOrDefaultAsync(p => p.Id == d.OponenteId);


                PersonagemHabilidade ph = await _context.TB_PERSONAGENS_HABILIDADES
                .Include(p => p.Habilidade).FirstOrDefaultAsync(phBusca => phBusca.HabilidadeId == d.HabilidadeId && phBusca.PersonagemId == d.AtacanteId );








                return BadRequest(ex.Message);

            }
        }
        
        [HttpPost("PersonagemRandom")]

        public async Task<IActionResult> Sorteio()
        {
            List<Personagem> personagens = await _context.Personagens.ToListAsync();

            int Sorteio = new Random().Next(personagens.Count);
            
            Personagem  p = personagens[Sorteio];


            string msg = string.Format("N° Sorteado {0}. Personagem: {1}",Sorteio,p.Nome);

            return Ok(msg)
        }


    }
}