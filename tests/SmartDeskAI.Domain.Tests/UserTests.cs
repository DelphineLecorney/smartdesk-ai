using SmartDeskAI.Domain.Entities;
using SmartDeskAI.Domain.Enums;
using SmartDeskAI.Domain.Exceptions;

namespace SmartDeskAI.Domain.Tests
{
    public class UserTests
    {
        [Fact]
        public void Un_utilisateur_invite_ne_peut_pas_se_connecter()
        {
            var user = User.Invite(Guid.NewGuid(), "test@example.com", UserRole.Agent);

            Assert.False(user.CanLogIn());
            Assert.Equal(UserStatus.Invited, user.Status);
        }

        [Fact]
        public void Activer_un_utilisateur_invite_le_rend_apte_a_se_connecter()
        {
            var user = User.Invite(Guid.NewGuid(), "test@example.com", UserRole.Agent);

            user.Activate();

            Assert.True(user.CanLogIn());
            Assert.NotNull(user.ActivatedAt);
        }

        [Fact]
        public void Activer_un_utilisateur_deja_actif_leve_une_exception()
        {
            var user = User.Invite(Guid.NewGuid(), "test@example.com", UserRole.Agent);
            user.Activate();

            Assert.Throws<InvalidUserStateTransitionException>(() => user.Activate());

        }

        [Fact]
        public void Desactiver_un_utilisateur_deja_desactive_leve_une_exception()
        {
            var user = User.Invite(Guid.NewGuid(), "test@example.com", UserRole.Agent);
            user.Activate();
            user.Deactivate();

            Assert.Throws<InvalidUserStateTransitionException>(() => user.Deactivate());
        }

        [Theory]
        [InlineData("")]
        [InlineData("pas-un-email")]
        [InlineData("manque-arobase.com")]
        public void Inviter_avec_un_email_invalide_leve_une_exception(string invalidEmail)
        {
            Assert.Throws<ArgumentException>(() => User.Invite(Guid.NewGuid(), invalidEmail, UserRole.Agent));
        }

        [Fact]
        public void Le_TenantId_ne_peut_pas_etre_vide()
        {
            Assert.Throws<ArgumentException>(() => User.Invite(Guid.Empty, "test@example.com", UserRole.Agent));
        }
    }
}
