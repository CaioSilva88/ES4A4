using System;
using Xunit;
using crud.Controllers;
using crud.Interfacecrud;
using Microsoft.AspNetCore.Mvc;
using crud.Data;
using crud.Models;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;

namespace XUNIT_crud
{
    public class Testecrud
    {

        public IList<Usuario> Usuarios;

        public readonly Mock<IRepository> context;
        public readonly UsuariosAPIController _controller;

        public Testecrud()
        {
            context = new Mock<IRepository>();

             Usuarios = new List<Usuario>()
            {
                new Usuario()
                {
                    ID = "5c5e1546a15743afa60a66f2d7e75f44",
                    FirstName = "Janaina",
                    SurName = "Crema",
                    Age = 44,
                    CreationTime = new DateTime(2020, 12, 30)
                },
                new Usuario()
                {
                ID = "6c5e1546a15743afa60a66f2d7e75f44",
                FirstName = "Kim",
                SurName = "Taehyung",
                Age = 25,
                CreationTime = new DateTime(2021, 01, 01)
                },
            new Usuario()
            {
                ID = "7c5e1546a15743afa60a66f2d7e75f44",
                FirstName = "Jeon",
                SurName = "Jungkook",
                Age = 23,
                CreationTime = new DateTime(2021, 01, 01)
            }

        };

            context
                .Setup(repo => repo.GetUsuarios())
                .ReturnsAsync(Usuarios.ToList());

            context
                 .Setup(repo => repo.GetUsuario(It.IsAny<string>()))
                .ReturnsAsync((string id) => Usuarios.SingleOrDefault(UsuariosController => UsuariosController.ID == id));
            context
                .Setup(repo => repo.PostUsuario(It.IsAny<Usuario>()))
                .Callback((Usuario Usuario) => Usuarios.Add(Usuario))
                .ReturnsAsync((Usuario Usuario) => Usuario);
            context
                .Setup(repo => repo.DeleteUsuario(It.IsAny<string>()))
                .ReturnsAsync((string id) => Usuarios.SingleOrDefault(UsuariosController => UsuariosController.ID == id))
                .Callback((string id) =>
                {
                    var delete = Usuarios.SingleOrDefault(UsuariosController => UsuariosController.ID == id);
                    Usuarios.Remove(delete);
                });
            context
                .Setup(repo => repo.PutUsuario(It.IsAny<string>(), It.IsAny<Usuario>()))
                .ReturnsAsync((string id, Usuario Usuario) => Usuarios.SingleOrDefault(Cadastroes => Cadastroes.ID == id));

            context
                .Setup(repo => repo.PatchUsuario(It.IsAny<string>(), It.IsAny<JsonPatchDocument<Usuario>>()))
                .Callback((string id, JsonPatchDocument<Usuario> patchUser) => {
                    var user = Usuarios.SingleOrDefault(x => x.ID == id);
                    if (user != null)
                    {
                        patchUser.ApplyTo(user);
                    }
                })
                .ReturnsAsync((string id, JsonPatchDocument<Usuario> patchUser) => Usuarios.SingleOrDefault(x => x.ID == id));
            context.SetupAllProperties();

            _controller = new UsuariosAPIController(context.Object);
        }






        [Fact]
        public async void GetCadastro_Return_OkResult()
        {
            //Get_IDPassed_OkResult
            //ByExistingIDPassed


            var data = await _controller.GetUsuario("5c5e1546a15743afa60a66f2d7e75f44");
            
            Assert.IsType<OkObjectResult>(data.Result);
        }

        [Fact]
        public async void Get_WhenCalled_ReturnsAllItems()
        {

            //Get_AllUsers_OkResult
            var usuarios = await _controller.GetUsuarios();

            Assert.IsType<OkObjectResult>(usuarios.Result);

        }

        [Fact]
        public async void GetById_UnknownGuidPassed_ReturnsNotFoundResult()
        {

            //Get_AnyUser_NotFoundResult
            var usuarioInexistente = await _controller.GetUsuario(Guid.NewGuid().ToString());

            Assert.IsType<NotFoundResult>(usuarioInexistente.Result);

            
        }


        [Fact]
        public async void PostCadastro_Return_BadRequestResult()
        {

            //act
            var user = new Usuario()
            {
                ID = "5c5e1546a15743afa60a66f2d7e75f44",
                FirstName = "Janaina",
                SurName = "Crema",
                Age = 44,
                CreationTime = new DateTime(2020, 12, 30)
            };
 
            _controller.ModelState.AddModelError("firstName", "O campo nome é obtigatório");
            var data = await _controller.PostUsuario(user);
            //Assert
            Assert.IsType<BadRequestObjectResult>(data.Result);

           


        }

        [Theory]
        [InlineData("6c5e1546a15743afa60a66f2d7e75f44")]
        [InlineData("7c5e1546a15743afa60a66f2d7e75f44")]
        public async void DeleteCadastro_Return_OkResult_TheoryMethod(string id)
        {

            //Act
            var data = await _controller.DeleteUsuario(id);
            //Assert
            Assert.IsType<OkResult>(data.Result);

        }

        [Fact]
        public async void Post_ValidObjectPassed_ReturnsCreatedResponse()
        {
            // Arrange
            Usuario user = new Usuario()
            {
                ID = Guid.NewGuid().ToString(),
                FirstName = "Hansol",
                SurName = "Vernon",
                Age = 25,
                CreationTime = DateTime.Now
            };

            // Act
            var UsuarioCriado = await _controller.PostUsuario(user);

            // Assert
            Assert.IsType<CreatedAtActionResult>(UsuarioCriado.Result);
        }







        [Fact]
        public async void DeleteCadastro_Return_OkResult()
        {
            //Delete_IDPassed_OkResult
            //ByExistingIDPassed

            //Act
            var data = await _controller.DeleteUsuario("5c5e1546a15743afa60a66f2d7e75f44");
            //Assert
            Assert.IsType<OkResult>(data.Result);

        }


        [Theory]
        [InlineData("84ea37d3-a5e4-4745-bc95-1aa4d866196c")]
        [InlineData("10ea37d3-a5e4-4745-bc95-1aa4d866196c")]
        [InlineData("90ea37d3-a5e4-4745-bc95-1aa4d866196c")]
        public async void Delete_UnknownGuidPassed_ReturnsNotFoundResponse_TheoryMethod(string id)
        {
            
            var notFound = await _controller.DeleteUsuario(id);
            Assert.IsType<NotFoundResult>(notFound.Result);
        }


        [Fact]
        public async void Delete_NotExistingGuidPassed_ReturnsNotFoundResponse()
        {

            //Delete_UnknownUser_NotFoundResult
            var user = Guid.NewGuid().ToString();
            var notFound = await _controller.DeleteUsuario(user);

            Assert.IsType<NotFoundResult>(notFound.Result);

        }


       







    }







}
